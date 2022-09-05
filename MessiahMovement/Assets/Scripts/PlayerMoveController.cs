using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Listens to keyboard inputs and moves/jumps the character
/// </summary>
/// <remarks>
/// There are several modes of movement:
/// 
/// On ground, inputs directly accelerate the player
/// 
/// While flying, inputs adjust velociy, so movement is less direct/more floaty
/// 
/// Also, there is a stamina meter that dictates will limit how much the player can flap their wings/glide
/// </remarks>
public class PlayerMoveController : MonoBehaviour
{
    #region Declaration Section

    public CharacterController Controller = null;

    public float KeyboardSpeed = 12f;
    public float MaxFlightSpeedPercent = 1.5f;

    private PlayerMoveController_Percents Percent_Ground = new PlayerMoveController_Percents() { Forward = 1f, Backward = 1f, Side = 1f };
    private PlayerMoveController_Percents Percent_Flap = new PlayerMoveController_Percents() { Forward = .25f, Backward = .5f, Side = .5f };
    private PlayerMoveController_Percents Percent_Glide = new PlayerMoveController_Percents() { Forward = .042f, Backward = .25f, Side = .25f };
    private PlayerMoveController_Percents Percent_Fall = new PlayerMoveController_Percents() { Forward = 0f, Backward = .167f, Side = .083f };

    public float JumpHeight_Ground = 1.2f;
    private float JumpHeight_Flap = .6f;

    // These are for the "foot" at the bottom of the character to see if it is airborn (controls when to reset falling velocity)
    public Transform GroundCheck;
    private float GroundDistance = .9f;     //TODO: Set this back to public once things are ironed out.  The radius of the cylinder is either .3 or .6, so it's possible for the things to catch.  May need to change to a capsule
    public LayerMask GroundMask;

    // The acceleration felt when free falling
    public float Gravity = -9.8f;

    private float Glide_PercentGravity = .25f;

    private readonly FlightStamina _stamina = new FlightStamina();

    private Vector3 _velocity_xz;
    private Vector3 _velocity_y;      // only the y portion is non zero

    private PlayerAudio _audio = null;

    #endregion

    private void Start()
    {
        _audio = new PlayerAudio(FindObjectOfType<SoundDefinitions>(), gameObject);
    }

    void Update()
    {
        float sideways = Input.GetAxis("Horizontal");
        float frontBack = Input.GetAxis("Vertical");
        bool isJumpPressed = Input.GetButtonDown("Jump");

        if (Physics.CheckSphere(GroundCheck.position, GroundDistance, GroundMask))
        {
            Update_OnGround(sideways, frontBack, isJumpPressed);
        }
        else
        {
            bool isJumpHeldDown = Input.GetButton("Jump");
            Update_Airborn(sideways, frontBack, isJumpPressed, isJumpHeldDown);
        }

        _audio.Update(Time.deltaTime);
    }

    private void Update_OnGround(float sideways, float frontBack, bool isJumpPressed)
    {
        // Run along xz plane
        float x = sideways * KeyboardSpeed * Percent_Ground.Side;
        float z = frontBack * KeyboardSpeed * Percent_Ground.GetFrontBack(frontBack);

        Vector3 move =
            (transform.right * x) +
            (transform.forward * z);

        Controller.Move(move * Time.deltaTime);

        _velocity_xz = move;        // the velocity is only looked at when they are airborn

        // Jump
        if (isJumpPressed && _stamina.Jump())
        {
            _velocity_y.y = Mathf.Sqrt(JumpHeight_Ground * -2f * Gravity);

            _audio.SetState(PlayerAudio_Action.WingFlap);
        }
        else
        {
            if (_velocity_y.y < 0f)        // this if negative is needed because if they just jumped, the velocity would still be upward, even though the foot is still detecting that we're grounded
            {
                //TODO: This takes too long to settle.  The foot's radius is too large.  If it's smaller, it misses.  So the problem is with that foot.  Make a second collider and detect when the physics engine says it's touching
                _velocity_y.y = -.2f;       // give it a small value down so the player doesn't hover just over the ground

                _stamina.Update(Time.deltaTime, FlightStamina_Activity.OnGround);
            }
            else
            {
                _stamina.Update(Time.deltaTime, FlightStamina_Activity.Falling);        // they might be falling or drifting.  Give the benefit of the doubt and use the less expensive of the two
            }
        }

        Controller.Move(_velocity_y * Time.deltaTime);

        // Set audio if they are on the ground
        if(_velocity_y.y <= 0f)
        {
            if (move.sqrMagnitude > 0)
            {
                _audio.SetState(PlayerAudio_Action.Walking);
            }
            else
            {
                _audio.SetState(PlayerAudio_Action.Nothing);
            }
        }
    }

    private void Update_Airborn(float sideways, float frontBack, bool isJumpPressed, bool isJumpHeldDown)
    {
        if (isJumpPressed && _stamina.Flap())        // flap will only reduce stamina if it returns true
        {
            Update_Airborn_Flap(sideways, frontBack);

            _audio.SetState(PlayerAudio_Action.WingFlap);
        }
        else if (isJumpHeldDown)
        {
            if (_stamina.Update(Time.deltaTime, FlightStamina_Activity.Gliding))
            {
                Update_Airborn_Glide(sideways, frontBack);
            }
            else
            {
                Update_Airborn_Fall(sideways, frontBack);
            }

            _audio.SetState(PlayerAudio_Action.Nothing);
        }
        else
        {
            _stamina.Update(Time.deltaTime, FlightStamina_Activity.Falling);

            Update_Airborn_Fall(sideways, frontBack);

            _audio.SetState(PlayerAudio_Action.Nothing);
        }
    }

    private void Update_Airborn_Flap(float sideways, float frontBack)
    {
        MoveXZVelocity(sideways, frontBack, Percent_Flap);      // not multiplying my delta time here so that more intantanious velocity changes will happen

        _velocity_y.y = Mathf.Sqrt(JumpHeight_Flap * -2f * Gravity);

        Controller.Move(_velocity_y * Time.deltaTime);
    }
    private void Update_Airborn_Glide(float sideways, float frontBack)
    {
        MoveXZVelocity(sideways * Time.deltaTime, frontBack * Time.deltaTime, Percent_Glide);

        _velocity_y.y += Gravity * Glide_PercentGravity * Time.deltaTime;

        Controller.Move(_velocity_y * Time.deltaTime);
    }
    private void Update_Airborn_Fall(float sideways, float frontBack)
    {
        MoveXZVelocity(sideways * Time.deltaTime, frontBack * Time.deltaTime, Percent_Fall);

        _velocity_y.y += Gravity * Time.deltaTime;

        Controller.Move(_velocity_y * Time.deltaTime);
    }

    private void MoveXZVelocity(float sideways, float frontBack, PlayerMoveController_Percents percents)
    {
        //float x = sideways * KeyboardSpeed * percents.Side * Time.deltaTime;
        //float z = frontBack * KeyboardSpeed * percents.GetFrontBack(frontBack) * Time.deltaTime;
        float x = sideways * KeyboardSpeed * percents.Side;
        float z = frontBack * KeyboardSpeed * percents.GetFrontBack(frontBack);

        Vector3 move =
            (transform.right * x) +
            (transform.forward * z);

        _velocity_xz += move;

        float maxSpeed = KeyboardSpeed * MaxFlightSpeedPercent;
        if(_velocity_xz.sqrMagnitude > maxSpeed * maxSpeed)
        {
            _velocity_xz = _velocity_xz.normalized * maxSpeed;
        }

        Controller.Move(_velocity_xz * Time.deltaTime);
    }
}

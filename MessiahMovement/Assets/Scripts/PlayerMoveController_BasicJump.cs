using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveController_BasicJump : MonoBehaviour
{
    public CharacterController Controller = null;

    //TODO: When they are airborn, don't let them change directions with the keyboard as easily
    //Not sure what the best approach would be.  Maybe character controller while touching the
    //ground, then switch to rigidbody while airborn
    public float KeyboardSpeed_Ground = 12f;

    //TODO: While airborn, need a reduced jump ability, but some kind of replenishing stamina.
    //So every time you flap your wings, the stamina drains a bit, but steadily replenishes
    //while not flapping wings
    public float JumpHeight_Ground = 3f;

    // These are for the "foot" at the bottom of the character to see if it is airborn (controls when to reset falling velocity)
    public Transform GroundCheck;
    public float GroundDistance = .2f;
    public LayerMask GroundMask;

    // These cause the player to fall toward the ground
    public float Gravity = -9.8f;
    private Vector3 _fallingVelocity;      // only the y portion is non zero

    void Update()
    {
        // Move from keyboard (and maybe also game controllers?)
        float x = Input.GetAxis("Horizontal") * KeyboardSpeed_Ground * Time.deltaTime;
        float z = Input.GetAxis("Vertical") * KeyboardSpeed_Ground * Time.deltaTime;

        Vector3 move =
            (transform.right * x) +
            (transform.forward * z);

        Controller.Move(move);

        // Apply Gravity
        bool isGrounded = Physics.CheckSphere(GroundCheck.position, GroundDistance, GroundMask);

        // Jump
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            _fallingVelocity.y = Mathf.Sqrt(JumpHeight_Ground * -2f * Gravity);
        }

        if (isGrounded && _fallingVelocity.y < 0)
        {
            _fallingVelocity.y = -.2f;       // give it a small value down so the player doesn't hover just over the ground
        }
        else
        {
            _fallingVelocity.y += Gravity * Time.deltaTime;
        }

        Controller.Move(_fallingVelocity * Time.deltaTime);     //NOTE: only store one delta to the velocity, but need to multiply here because it needs to be squared
    }
}

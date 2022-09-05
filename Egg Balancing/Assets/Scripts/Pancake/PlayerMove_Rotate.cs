using PerfectlyNormalUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove_Rotate : MonoBehaviour
{
    public float ACCEL_ROLL = 6f;
    public float ACCEL_PITCH = 6f;
    public float ACCEL_YAW = 6f;

    public GameObject RigidBodyObject;
    private Rigidbody _body = null;

    private float _yaw = 0f;
    private float _pitch = 0f;
    private float _roll = 0f;

    void Start()
    {
        _body = RigidBodyObject.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // Yaw
        float yaw = 0f;

        if (Keyboard.current.leftArrowKey.isPressed)
            yaw -= 1f;

        if (Keyboard.current.rightArrowKey.isPressed)
            yaw += 1f;

        _yaw = yaw;

        // Pitch
        float pitch = 0f;

        if (Keyboard.current.upArrowKey.isPressed)
            pitch += 1f;

        if (Keyboard.current.downArrowKey.isPressed)
            pitch -= 1f;

        _pitch = pitch;

        // Roll
        float roll = 0f;

        if (Keyboard.current.qKey.isPressed)
            roll += 1f;

        if (Keyboard.current.eKey.isPressed)
            roll -= 1f;

        _roll = roll;
    }

    private void FixedUpdate()
    {
        // Yaw
        if (!_yaw.IsNearZero())
            _body.AddTorque((_body.rotation * Vector3.up) * (_yaw * ACCEL_YAW), ForceMode.Acceleration);

        // Pitch
        if (!_pitch.IsNearZero())
            _body.AddTorque((_body.rotation * Vector3.right) * (_pitch * ACCEL_PITCH), ForceMode.Acceleration);

        // Roll
        if (!_roll.IsNearZero())
            _body.AddTorque((_body.rotation * Vector3.forward) * (_roll * ACCEL_PITCH), ForceMode.Acceleration);
    }
}

using PerfectlyNormalUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove_Translate : MonoBehaviour
{
    public GameObject RigidBodyObject;
    private Rigidbody _body = null;

    public float ACCEL_HORZ = 12;
    public float ACCEL_VERT = 16;

    private float _frontBack = 0;
    private float _sideways = 0;
    private bool _isJumpPressed = false;

    private void Start()
    {
        _body = RigidBodyObject.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        //_frontBack = Input.GetAxis("Vertical");
        //_sideways = Input.GetAxis("Horizontal");
        //_isJumpPressed = Input.GetButton("Jump");


        // Front/Back
        float front_back = 0f;

        if (Keyboard.current.sKey.isPressed)
            front_back -= 1f;

        if (Keyboard.current.wKey.isPressed)
            front_back += 1f;

        _frontBack = front_back;

        // Sideways
        float sideways = 0f;

        if (Keyboard.current.aKey.isPressed)
            sideways -= 1f;

        if (Keyboard.current.dKey.isPressed)
            sideways += 1f;

        _sideways = sideways;

        // Jump
        _isJumpPressed = Keyboard.current.spaceKey.isPressed;
    }

    private void FixedUpdate()
    {
        Vector3 forward = _frontBack.IsNearZero() ?
            Vector3.zero :
            (_body.rotation * Vector3.forward) * (_frontBack * ACCEL_HORZ);

        Vector3 right = _sideways.IsNearZero() ?
            Vector3.zero :
            (_body.rotation * Vector3.right) * (_sideways * ACCEL_HORZ);

        Vector3 up = _isJumpPressed ?
            (_body.rotation * Vector3.up) * (1 * ACCEL_VERT) :
            Vector3.zero;

        _body.AddForce(forward + right + up, ForceMode.Acceleration);
    }
}

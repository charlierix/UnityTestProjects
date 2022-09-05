using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove_Ironman : MonoBehaviour
{
    public GameObject RigidBodyObject;
    private Rigidbody _body = null;

    public float OFFSET_Y = 0.2f;
    public float OFFSET_CENTER_X = 0.15f;
    public float OFFSET_CORNER_X = 0.1f;
    public float OFFSET_CORNER_Z = 0.08f;

    public float ACCEL_VERT = 9;

    private bool _pressed_left_front = false;
    private bool _pressed_left_center = false;
    private bool _pressed_left_rear = false;

    private bool _pressed_right_front = false;
    private bool _pressed_right_center = false;
    private bool _pressed_right_rear = false;

    private void Start()
    {
        _body = RigidBodyObject.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        _pressed_left_front = Keyboard.current.numpad7Key.isPressed;
        _pressed_left_center = Keyboard.current.numpad4Key.isPressed;
        _pressed_left_rear = Keyboard.current.numpad1Key.isPressed;

        _pressed_right_front = Keyboard.current.numpad9Key.isPressed;
        _pressed_right_center = Keyboard.current.numpad6Key.isPressed;
        _pressed_right_rear = Keyboard.current.numpad3Key.isPressed;
    }

    private void FixedUpdate()
    {
        // Left
        if (_pressed_left_front)
            AddForce(-OFFSET_CORNER_X, OFFSET_CORNER_Z);

        if (_pressed_left_center)
            AddForce(-OFFSET_CENTER_X, 0);

        if (_pressed_left_rear)
            AddForce(-OFFSET_CORNER_X, -OFFSET_CORNER_Z);

        // Right
        if (_pressed_right_front)
            AddForce(OFFSET_CORNER_X, OFFSET_CORNER_Z);

        if (_pressed_right_center)
            AddForce(OFFSET_CENTER_X, 0);

        if (_pressed_right_rear)
            AddForce(OFFSET_CORNER_X, -OFFSET_CORNER_Z);
    }

    private void AddForce(float x, float z)
    {
        Vector3 dir = _body.rotation * new Vector3(0, ACCEL_VERT, 0);

        Vector3 pos = _body.rotation * new Vector3(x, OFFSET_Y, z);
        pos += _body.position;

        _body.AddForceAtPosition(dir, pos, ForceMode.Acceleration);
    }
}

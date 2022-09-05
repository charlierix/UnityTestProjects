using PerfectlyNormalUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRMovment_Translate : MonoBehaviour
{
    public float ACCEL = 18;

    public GameObject RigidBodyObject;
    private Rigidbody _body = null;

    public ControllerListener Controllers;

    void Start()
    {
        _body = RigidBodyObject.GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        float x = Controllers.Thumbstick_X_Left;
        float y = Controllers.Thumbstick_Y_Left;

        if (x.IsNearZero() && y.IsNearZero())
            return;

        Vector3 forward = _body.transform.forward;
        Vector3 right = _body.transform.right;

        forward = Vector3.ProjectOnPlane(forward, Vector3.up).normalized;
        right = Vector3.ProjectOnPlane(right, Vector3.up).normalized;

        forward *= y * ACCEL;
        right *= x * ACCEL;

        _body.AddForce(forward + right, ForceMode.Acceleration);
    }
}

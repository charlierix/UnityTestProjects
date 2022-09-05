using PerfectlyNormalUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRMovement_Rotate : MonoBehaviour
{
    public float TORQUE = 6;

    public GameObject RigidBodyObject;
    private Rigidbody _body = null;

    public ControllerListener Controllers;

    void Start()
    {
        _body = RigidBodyObject.GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        float x = Controllers.Thumbstick_X_Right;

        if (x.IsNearZero())
            return;

        _body.AddTorque(Vector3.up * (TORQUE * x), ForceMode.Acceleration);
    }
}

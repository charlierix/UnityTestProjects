using PerfectlyNormalUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRThruster_TriggerBoost : MonoBehaviour
{
    public float OFFSET_Y = 0f; // 0.2f;        // if there's an offset, then it creates unwanted torques when pointing the controllers other than straight down
    public float OFFSET_X = 0.07f;

    public float ACCEL = 12;

    public GameObject RigidBodyObject;
    private Rigidbody _body = null;

    public Transform XROrigin;      // this is the same as RigidBodyObject, just being explicit to match the other classes
    public Transform BodyCollider;
    public Transform LeftController;
    public Transform RightController;

    public ControllerListener Controllers;
    public FlightStartStopTracker StartStopTracker;
    public FlightControllerValues FlightControllerValues;

    private void Start()
    {
        _body = RigidBodyObject.GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (!StartStopTracker.IsInFlight)
            return;

        float left = Controllers.Trigger_Left;
        if (!left.IsNearZero())
            AddAccel(left * ACCEL, false);

        float right = Controllers.Trigger_Right;
        if (!right.IsNearZero())
            AddAccel(right * ACCEL, true);
    }

    private void AddAccel(float accel, bool is_right)
    {
        Vector3 point = new Vector3(is_right ? OFFSET_X : -OFFSET_X, OFFSET_Y, 0);
        point = _body.rotation * point;     // This needs to be rotated into world coords

        point = BodyCollider.position + point;

        //---------- this would be straight down thrust only ----------
        //Vector3 direction = new Vector3(0, accel, 0);
        //direction = _body.rotation * direction;
        //-------------------------------------------------------------

        Vector3 direction = is_right ?
            FlightControllerValues.Up_Left :
            FlightControllerValues.Up_Right;

        direction *= accel;

        _body.AddForceAtPosition(direction, point, ForceMode.Acceleration);
    }
}

using PerfectlyNormalUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: deflect based on wrist orientation
//TODO: the input circles shouldn't have a 1:1 with output.  For example a high -z and small -y may create strong torque along x

public class VRThruster_DiscDown : MonoBehaviour
{
    public float OFFSET_Y = 0f;     // 0.2f;        // being offset from center just creates unwanted torques
    public float OFFSET_X = 0.15f;

    public float SCALE_RADIUS = 0.1f;
    public float SCALE_ACCEL = 0.15f;        // expected distance from plane hand needs to be to get max thrust

    public float ACCEL_UP = 7;
    public float ACCEL_DOWN = 7;

    public GameObject RigidBodyObject;
    private Rigidbody _body = null;

    public Transform XROrigin;
    public Transform BodyCollider;
    public Transform LeftController;
    public Transform RightController;

    public FlightStartStopTracker StartStopTracker;

    private Vector3 _plane_left;
    public Vector3 Plane_Left => _plane_left;       // exposed for the hud to draw

    private Vector3 _plane_right;
    public Vector3 Plane_Right => _plane_right;

    private void Start()
    {
        _body = RigidBodyObject.GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (!StartStopTracker.IsInFlight)
            return;

        Vector3 hand_left = XROrigin.InverseTransformPoint(LeftController.position);     // xrorigin could be flying around, rotated.  The positions need to be in room space
        Vector3 hand_right = XROrigin.InverseTransformPoint(RightController.position);

        // Project the hands onto the center's plane
        _plane_left = Math3D.GetClosestPoint_Plane_Point(new Plane(Vector3.up, StartStopTracker.Center_Left), hand_left);
        _plane_right = Math3D.GetClosestPoint_Plane_Point(new Plane(Vector3.up, StartStopTracker.Center_Right), hand_right);

        // Turn that into an acceleration (relative to the thruster)
        var accel_left = GetAccel(hand_left, _plane_left, StartStopTracker.Center_Left, false);
        var accel_right = GetAccel(hand_right, _plane_right, StartStopTracker.Center_Right, true);

        if (!accel_left.accel.IsNearZero())
            AddForce(accel_left.point, accel_left.accel);

        if (!accel_right.accel.IsNearZero())
            AddForce(accel_right.point, accel_right.accel);
    }

    private void AddForce(Vector3 point, Vector3 accel)
    {
        point = _body.rotation * point;     // This needs to be rotated into world coords
        point = BodyCollider.position + point;

        accel = _body.rotation * accel;

        _body.AddForceAtPosition(accel, point, ForceMode.Acceleration);
    }

    private (Vector3 point, Vector3 accel) GetAccel(Vector3 hand, Vector3 plane_hit, Vector3 plane_center, bool is_right)
    {
        Vector3 direction = hand - plane_hit;

        //if (Vector3.Dot(direction, Vector3.up) > 0)
        //    return GetAccel_Brake(hand, plane_hit, direction, is_right);        // The brakes just feel unnatural
        //else
        //    return GetAccel_Increase(hand, plane_hit, plane_center, direction, is_right);

        return GetAccel_Increase(hand, plane_hit, plane_center, direction, is_right);
    }

    private (Vector3 point, Vector3 accel) GetAccel_Increase(Vector3 hand, Vector3 plane_hit, Vector3 plane_center, Vector3 direction, bool is_right)
    {
        Vector3 offset_plane = (plane_hit - plane_center) * SCALE_RADIUS;
        Vector3 thrust_point = new Vector3(is_right ? OFFSET_X : -OFFSET_X, OFFSET_Y, 0) + offset_plane;

        Vector3 accel = direction / SCALE_ACCEL * ACCEL_DOWN;
        accel = -accel;     // when pulling down, the thrust vector points up

        return (thrust_point, accel);
    }

    // ------------ unused ------------
    private (Vector3 point, Vector3 accel) GetAccel_Brake(Vector3 hand, Vector3 plane_hit, Vector3 direction, bool is_right)
    {

        // everything above the disc needs to act like drag.  Not adding additional forces, just drag strength relative to current velocity
        // it will be interesting, because the drag will be applied


        // first attempt just apply an accel directly against current velocity

        Vector3 velocity = XROrigin.InverseTransformDirection(_body.velocity);
        float speed = velocity.magnitude;

        float thrust_magnitude = direction.magnitude / SCALE_ACCEL * ACCEL_UP;
        //if (thrust_magnitude > speed)
        //    thrust_magnitude = speed;

        Vector3 accel = velocity * (-thrust_magnitude / speed);     // turn velocity into a unit vector, then multiply by drag amount

        //Vector3 thrust_point = new Vector3(is_right ? OFFSET_X : -OFFSET_X, OFFSET_Y, 0);       // not doing anything with hand's plane offset from center
        Vector3 thrust_point = Vector3.zero;        // anything away from centerline causes unwanted tumbling

        return (thrust_point, accel);
    }
    private (Vector3 point, Vector3 accel) GetAccel_Increase_DOWNONLY(Vector3 hand, Vector3 plane_hit, Vector3 plane_center, Vector3 direction, bool is_right)
    {
        Vector3 offset_plane = (plane_hit - plane_center) * SCALE_RADIUS;
        Vector3 thrust_point = new Vector3(is_right ? OFFSET_X : -OFFSET_X, OFFSET_Y, 0) + offset_plane;

        float thrust_magnitude = direction.magnitude / SCALE_ACCEL * ACCEL_DOWN;

        Vector3 accel = new Vector3(0, thrust_magnitude, 0);

        // This shouldn't be combined with disc thrust, the magnitude needs a different calculation
        //Vector3 accel = is_right ?
        //    XROrigin.TransformDirection(StartStopTracker.GetHandUp(StartStopTracker.Up_Right, RightController)) :
        //    XROrigin.TransformDirection(StartStopTracker.GetHandUp(StartStopTracker.Up_Left, LeftController));

        //accel *= thrust_magnitude;

        return (thrust_point, accel);
    }
}

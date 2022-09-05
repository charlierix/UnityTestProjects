using PerfectlyNormalUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Same as VRThruster_DiscDown, but the thrust gets deflected based on wrist orientation
/// </summary>
public class VRThruster_DiscDown2 : MonoBehaviour
{
    public float OFFSET_X = 0.08f;

    //TODO: have a way to exaggerate input distances when the controller is forward of center (along +Z).  The forearm naturally pivots
    //down and back, but is very difficult to go forward
    //
    //on second thought, leave this class alone and create a new class that applies drag when arms go forward

    public float SCALE_RADIUS = 0.1f;
    public float SCALE_ACCEL = 0.15f;        // expected distance from plane hand needs to be to get max thrust

    public float WRIST_ROTATE_PERCENT = 0.33f;

    public float ACCEL = 8;

    public GameObject RigidBodyObject;
    private Rigidbody _body = null;

    public Transform XROrigin;
    public Transform BodyCollider;
    public Transform LeftController;
    public Transform RightController;

    public FlightStartStopTracker StartStopTracker;
    public FlightControllerValues FlightControllerValues;

    void Start()
    {
        _body = RigidBodyObject.GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (!StartStopTracker.IsInFlight)
            return;

        // Turn that into an acceleration (relative to the thruster)
        var accel_left = GetAccel(FlightControllerValues.Hand_Left, FlightControllerValues.Plane_Left, FlightControllerValues.Up_Left, StartStopTracker.Center_Left, false);
        var accel_right = GetAccel(FlightControllerValues.Hand_Right, FlightControllerValues.Plane_Right, FlightControllerValues.Up_Right, StartStopTracker.Center_Right, true);

        if (!accel_left.accel.IsNearZero())
            AddForce(accel_left.point, accel_left.accel);

        if (!accel_right.accel.IsNearZero())
            AddForce(accel_right.point, accel_right.accel);
    }

    private (Vector3 point, Vector3 accel) GetAccel(Vector3 hand, Vector3 plane_hit, Vector3 controller_up, Vector3 plane_center, bool is_right)
    {
        Vector3 direction = hand - plane_hit;
        direction = RotateDirection(direction, controller_up);

        Vector3 offset_plane = (plane_hit - plane_center) * SCALE_RADIUS;
        Vector3 thrust_point = new Vector3(is_right ? OFFSET_X : -OFFSET_X, 0, 0) + offset_plane;

        Vector3 accel = direction / SCALE_ACCEL * ACCEL;
        accel = -accel;     // when pulling down, the thrust vector points up

        return (thrust_point, accel);
    }

    private Vector3 RotateDirection(Vector3 vert_dir, Vector3 controller_up)
    {
        Quaternion quat = Quaternion.FromToRotation(Vector3.up, controller_up);
        if (WRIST_ROTATE_PERCENT.IsNearValue(1))
            return quat * vert_dir;

        //quat = Quaternion.Slerp(Quaternion.identity, quat, WRIST_ROTATE_PERCENT);     // this might work, but I don't trust rotating from identity

        quat.ToAngleAxis(out float angle, out Vector3 axis);
        quat = Quaternion.AngleAxis(angle * WRIST_ROTATE_PERCENT, axis);

        return quat * vert_dir;
    }

    private void AddForce(Vector3 point, Vector3 accel)
    {
        point = _body.rotation * point;     // This needs to be rotated into world coords
        point = BodyCollider.position + point;

        accel = _body.rotation * accel;

        _body.AddForceAtPosition(accel, point, ForceMode.Acceleration);
    }
}

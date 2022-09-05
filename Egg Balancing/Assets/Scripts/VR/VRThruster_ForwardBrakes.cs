using PerfectlyNormalUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Applies drag when arms go forward
/// </summary>
public class VRThruster_ForwardBrakes : MonoBehaviour
{
    public float SCALE_RADIUS = 0.1f;       // the distances are multiplied by this before other calculations are performed (otherwise, the values would get large too quickly, too sensitive)
    public float SCALE_ACCEL = 0.05f;        // expected distance from center hand needs to be to get max thrust
    public float DEAD_ZONE = 0.1f;      // how far the hand needs to move forward before brakes are considered (the actual distance is multiplied by SCALE_RADIUS first)

    public float ACCEL = 12;
    public float TORQUE = 6;

    public GameObject RigidBodyObject;
    private Rigidbody _body = null;

    public Transform XROrigin;
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

        float percent_left = GetDragPercent(FlightControllerValues.Plane_Left, StartStopTracker.Center_Left);
        float percent_right = GetDragPercent(FlightControllerValues.Plane_Right, StartStopTracker.Center_Right);

        float percent = Mathf.Clamp(percent_left + percent_right, 0, 1);

        if (percent.IsNearZero())
            return;

        ApplyDrag_Translate(percent);
        ApplyDrag_Rotate(percent);
    }

    private float GetDragPercent(Vector3 plane_hit, Vector3 plane_center)
    {
        // Get the component along forward
        Vector3 along_forward = Vector3.Project(plane_hit - plane_center, StartStopTracker.Forward);

        if (Vector3.Dot(along_forward, StartStopTracker.Forward) < 0)
            return 0;

        float distance_unscaled = along_forward.magnitude;

        float distance_scaled = distance_unscaled * SCALE_RADIUS;
        if (distance_scaled < DEAD_ZONE)
            return 0;

        distance_unscaled -= DEAD_ZONE;      // start at zero when at the edge of the dead zone

        return Mathf.Clamp(distance_unscaled / SCALE_ACCEL, 0, 1);
    }

    private void ApplyDrag_Translate(float percent)
    {
        float speed = _body.velocity.magnitude;

        float drag = ACCEL * percent;
        if (drag > speed)
            drag = speed;

        Vector3 accel = _body.velocity / speed * drag;
        accel = -accel;

        _body.AddForce(accel, ForceMode.Acceleration);
    }
    private void ApplyDrag_Rotate(float percent)
    {
        float speed = _body.angularVelocity.magnitude;

        float drag = TORQUE * percent;
        if (drag > speed)
            drag = speed;

        Vector3 torque = _body.angularVelocity / speed * drag;
        torque = -torque;

        _body.AddTorque(torque, ForceMode.Acceleration);
    }
}

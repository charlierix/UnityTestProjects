using PerfectlyNormalUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRMovement_SphereHandSteer : MonoBehaviour
{
    public float OFFSET_Y = 0f;
    public float OFFSET_X = 0.18f;

    public float SCALE_ACCEL = 0.15f;        // expected distance from center point needs to be to get max thrust

    public float ACCEL = 6;

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

        ApplyAccel(new Vector3(-OFFSET_X, OFFSET_Y, 0), FlightControllerValues.Hand_Left, FlightControllerValues.Plane_Left, FlightControllerValues.Up_Left);
        ApplyAccel(new Vector3(OFFSET_X, OFFSET_Y, 0), FlightControllerValues.Up_Right, FlightControllerValues.Plane_Right, FlightControllerValues.Up_Right);
    }

    private void ApplyAccel_ABSDIST(Vector3 at_point, Vector3 center, Vector3 hand_pos, Vector3 up)
    {
        float distance = (hand_pos - center).magnitude;

        float accel_magnitude = distance / SCALE_ACCEL * ACCEL;

        Vector3 accel_dir = up;
        accel_dir = Vector3.ProjectOnPlane(accel_dir, Vector3.up);      // only want the portion along the xz plane.  Other classes will handle lift forces

        at_point = _body.rotation * at_point;     // This needs to be rotated into world coords
        at_point = BodyCollider.position + at_point;

        accel_dir = XROrigin.TransformDirection(accel_dir);

        _body.AddForceAtPosition(accel_dir * accel_magnitude, at_point, ForceMode.Acceleration);
    }
    private void ApplyAccel(Vector3 at_point, Vector3 hand_pos, Vector3 plane_pos, Vector3 up)
    {
        float distance = (hand_pos - plane_pos).magnitude;
        //float distance = (hand_pos - center).magnitude;

        float accel_magnitude = distance / SCALE_ACCEL * ACCEL;

        Vector3 accel_dir = up;
        accel_dir = Vector3.ProjectOnPlane(accel_dir, Vector3.up);      // only want the portion along the xz plane.  Other classes will handle lift forces

        at_point = _body.rotation * at_point;     // This needs to be rotated into world coords
        at_point = BodyCollider.position + at_point;

        accel_dir = XROrigin.TransformDirection(accel_dir);

        _body.AddForceAtPosition(accel_dir * accel_magnitude, at_point, ForceMode.Acceleration);
    }
}

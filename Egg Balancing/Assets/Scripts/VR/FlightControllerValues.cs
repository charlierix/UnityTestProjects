using PerfectlyNormalUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// These are a few calculated values that are used my multiple worker classes
/// </summary>
public class FlightControllerValues : MonoBehaviour
{
    public Transform XROrigin;
    public Transform LeftController;
    public Transform RightController;

    public FlightStartStopTracker StartStopTracker;

    private Vector3 _hand_left;
    public Vector3 Hand_Left => _hand_left;

    private Vector3 _hand_right;
    public Vector3 Hand_Right => _hand_right;

    private Vector3 _plane_left;
    public Vector3 Plane_Left => _plane_left;

    private Vector3 _plane_right;
    public Vector3 Plane_Right => _plane_right;

    private Vector3 _up_left;
    public Vector3 Up_Left => _up_left;

    private Vector3 _up_right;
    public Vector3 Up_Right => _up_right;

    void Update()
    {
        if (!StartStopTracker.IsInFlight)
            return;

        // Pull the hands into room coords
        _hand_left = XROrigin.InverseTransformPoint(LeftController.position);     // xrorigin could be flying around, rotated.  The positions need to be in room space
        _hand_right = XROrigin.InverseTransformPoint(RightController.position);

        // Project the hands onto the center's plane
        _plane_left = Math3D.GetClosestPoint_Plane_Point(new Plane(Vector3.up, StartStopTracker.Center_Left), _hand_left);
        _plane_right = Math3D.GetClosestPoint_Plane_Point(new Plane(Vector3.up, StartStopTracker.Center_Right), _hand_right);

        // Figure out how the controllers are oriented (relative to how they were pointed at time of starting flight - just in case
        // the player naturally holds their controllers funny)
        _up_left = StartStopTracker.GetHandUp(StartStopTracker.Up_Left, LeftController);
        _up_right = StartStopTracker.GetHandUp(StartStopTracker.Up_Right, RightController);
    }
}

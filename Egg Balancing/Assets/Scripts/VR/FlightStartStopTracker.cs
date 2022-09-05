using PerfectlyNormalUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// To start flight:  Hold arms straight out and press both thumb pads at the same time.  The points where the controllers were is the zero point
/// To stop flight: Push both thumb pads at the same time
/// </summary>
public class FlightStartStopTracker : MonoBehaviour
{
    private struct Points_VR
    {
        public Vector3 pos_left { get; set; }
        public Vector3 pos_right { get; set; }
        public Vector3 pos_head { get; set; }       //TODO: use the position of the neck instead
        public Vector3 head_forward { get; set; }
        public Vector3 up_left { get; set; }
        public Vector3 up_right { get; set; }
    }
    private struct Points_Pass
    {
        public Vector3 pos_left { get; set; }       // calculated average point (left and right are mirror points)
        public Vector3 pos_right { get; set; }
        public Vector3 center_point { get; set; }       // probably not needed outside of debug logging
        public Vector3 forward { get; set; }
    }

    private const string CATEGORY_LEFT = "left";
    private const string CATEGORY_RIGHT = "right";
    private const string CATEGORY_HEAD = "head";
    private const string CATEGORY_PASS = "pass";

    public bool ShouldLog = false;

    public Transform XROrigin;
    public Transform HeadController;
    public Transform LeftController;
    public Transform RightController;

    public ControllerListener ControllerListener;

    public bool IsInFlight { get; private set; }

    // These get populated when entering flight
    public Vector3 Center_Left { get; private set; }        // if the user's hand is at this point, there should be no force applied
    public Vector3 Center_Right { get; private set; }
    public Vector3 MidPoint { get; private set; }
    public Vector3 Forward { get; private set; }
    public Vector3 Up_Left { get; private set; }        // this is vector3.up rotated by the left hand's rotation
    public Vector3 Up_Right { get; private set; }

    private DateTime _down_left = new DateTime(2000, 1, 1);
    private DateTime _down_right = new DateTime(2000, 1, 1);
    private DateTime _switch_time = new DateTime(2000, 1, 1);

    private static Quaternion _quat180_y = Quaternion.Euler(0, 180, 0);

    private void Start()
    {
        ControllerListener.Down_Left_Touchpad.AddListener(() => _down_left = DateTime.UtcNow);
        ControllerListener.Down_Right_Touchpad.AddListener(() => _down_right = DateTime.UtcNow);
    }

    private void Update()
    {
        const float TIME_IGNORE = 1;
        const float TIME_GRACE = 0.15f;
        const float EXIT_YDIFF = 0.08f;
        //const float EXIT_HAND_HEAD_DIST = 0.2f;      // the head's sensor is on the forehead, so distance will change based on looking left/right
        const float EXIT_HAND_AVG_DIST = 0.035f;

        DateTime now = DateTime.UtcNow;

        if ((now - _down_left).TotalSeconds > TIME_GRACE || (now - _down_right).TotalSeconds > TIME_GRACE)
            return;     // buttons weren't recently pressed at the same time

        if ((now - _switch_time).TotalSeconds < TIME_IGNORE)
            return;     // just entered/exited flight, need to wait longer

        if (IsInFlight)
        {
            IsInFlight = false;     // pushing both buttons at the same time is the only requirement for exiting flight
            _switch_time = now;
            return;
        }

        var vr = GetVRPositions();

        if (Mathf.Abs(vr.pos_left.y - vr.pos_right.y) > EXIT_YDIFF)
            return;     // not in the same xz plane

        // this is failing when looking too much outside of directly forward
        //if (Mathf.Abs((vr.pos_left - vr.pos_head).sqrMagnitude - (vr.pos_right - vr.pos_head).sqrMagnitude) > EXIT_HAND_HEAD_DIST * EXIT_HAND_HEAD_DIST)
        //    return;     // hands are different distance from the head

        var avg_leftright = GetAveragePoints(vr);

        if ((vr.pos_left - avg_leftright.pos_left).sqrMagnitude > EXIT_HAND_AVG_DIST * EXIT_HAND_AVG_DIST)
            return;     // not close enough to avg point

        //if ((vr.pos_right - avg_leftright.pos_right).sqrMagnitude > EXIT_HAND_AVG_DIST * EXIT_HAND_AVG_DIST)        // this check isn't needed, since avg is calculated with left and right (if left is inside average, right will be as well)
        //    return;

        Center_Left = avg_leftright.pos_left;
        Center_Right = avg_leftright.pos_right;
        MidPoint = avg_leftright.center_point;
        Forward = avg_leftright.forward;
        Up_Left = vr.up_left;
        Up_Right = vr.up_right;
        _switch_time = now;
        IsInFlight = true;
    }

    public void StopFlight()
    {
        if (IsInFlight)
        {
            IsInFlight = false;
            _switch_time = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// This returns Vector3.up rotated relative to how the hand was when starting flight
    /// This is in model/room coords (not world coords)
    /// </summary>
    public Vector3 GetHandUp(Vector3 up_at_start, Transform hand)
    {
        Vector3 up_rotated = XROrigin.InverseTransformDirection(hand.rotation * Vector3.up);

        Quaternion quat = Quaternion.FromToRotation(up_at_start, up_rotated);

        return quat * Vector3.up;
    }

    private Points_Pass GetAveragePoints(Points_VR vr)
    {
        var log = new DebugLogger(@"d:\temp\flight stopstart", ShouldLog);

        if (log.Logging_Enabled)       //NOTE: if log is disabled, it's pretty optimal (first statement of every function is if(!enabled) return), but there's no point converting colors if they aren't used
        {
            log.DefineCategory(CATEGORY_LEFT, UtilityUnity.ColorFromHex("A33F37"));
            log.DefineCategory(CATEGORY_RIGHT, UtilityUnity.ColorFromHex("23A157"));
            log.DefineCategory(CATEGORY_HEAD, UtilityUnity.ColorFromHex("EBEBEB"));
            log.DefineCategory(CATEGORY_PASS, UtilityUnity.ColorFromHex("768FDE"));

            log.NewFrame("initial");
            log.Add_Dot(vr.pos_left, CATEGORY_LEFT);
            log.Add_Dot(vr.pos_right, CATEGORY_RIGHT);
            log.Add_Dot(vr.pos_head, CATEGORY_HEAD);
            log.Add_Line(vr.pos_head, vr.pos_head + vr.head_forward, CATEGORY_HEAD);
            log.WriteLine_Frame($"left: {vr.pos_left.ToStringSignificantDigits(3)}");
            log.WriteLine_Frame($"right: {vr.pos_right.ToStringSignificantDigits(3)}");
            log.WriteLine_Frame($"head: {vr.pos_head.ToStringSignificantDigits(3)}");
            log.WriteLine_Frame($"forward: {vr.head_forward.ToStringSignificantDigits(3)}");

            float diff_left = (vr.pos_left - vr.pos_head).magnitude;
            float diff_right = (vr.pos_right - vr.pos_head).magnitude;
            log.WriteLine_Global($"dist left to head: {diff_left}");
            log.WriteLine_Global($"dist right to head: {diff_right}");
            log.WriteLine_Global($"diff dist: {Mathf.Abs(diff_right - diff_left)}");
            log.WriteLine_Global("");
            log.WriteLine_Global($"y diff: {Mathf.Abs(vr.pos_left.y - vr.pos_right.y)}");
        }

        log.NewFrame("180 rotation");
        var pass = GetPoints_FirstPass(log, vr);
        pass = AdjustCenter(vr.pos_left, vr.pos_right, pass);

        log.NewFrame("mirror rotation");
        pass = GetPoints_SecondPass(log, vr, pass);
        pass = AdjustCenter(vr.pos_left, vr.pos_right, pass);

        float dist_left_sqr = (vr.pos_left - pass.pos_left).sqrMagnitude;
        float dist_right_sqr = (vr.pos_right - pass.pos_right).sqrMagnitude;      // probably only need one

        if (log.Logging_Enabled)
        {
            log.NewFrame("final");
            log.Add_Dot(vr.pos_left, CATEGORY_LEFT);
            log.Add_Dot(vr.pos_right, CATEGORY_RIGHT);
            log.Add_Dot(vr.pos_head, CATEGORY_HEAD);
            log.Add_Line(vr.pos_head, vr.pos_head + vr.head_forward, CATEGORY_HEAD);
            log.Add_Dot(pass.pos_left, CATEGORY_PASS);
            log.Add_Dot(pass.pos_right, CATEGORY_PASS);
            log.Add_Line(pass.center_point, pass.center_point + pass.forward, CATEGORY_PASS);

            log.WriteLine_Frame($"dist to avg - left: {Mathf.Sqrt(dist_left_sqr)}");
            log.WriteLine_Frame($"dist to avg - right: {Mathf.Sqrt(dist_right_sqr)}");

            log.Save();
        }

        return pass;
    }

    private Points_VR GetVRPositions()
    {
        return new Points_VR()
        {
            pos_left = XROrigin.InverseTransformPoint(LeftController.position),     // xrorigin could be flying around, rotated.  The positions need to be in room space
            pos_right = XROrigin.InverseTransformPoint(RightController.position),
            pos_head = XROrigin.InverseTransformPoint(HeadController.position),
            head_forward = XROrigin.InverseTransformDirection(HeadController.forward),
            up_left = XROrigin.InverseTransformDirection(LeftController.rotation * Vector3.up),
            up_right = XROrigin.InverseTransformDirection(RightController.rotation * Vector3.up),
        };
    }

    /// <summary>
    /// This rotates 180 around the Y axis, takes the mid point, finds line and forward based on that
    /// </summary>
    /// <remarks>
    /// This is inacurrate for the final solution, but is good for finding the forward direction
    /// </remarks>
    private static Points_Pass GetPoints_FirstPass(DebugLogger log, Points_VR vr)
    {
        log.Add_Dot(vr.pos_left, CATEGORY_LEFT);
        log.Add_Dot(vr.pos_right, CATEGORY_RIGHT);
        log.Add_Dot(vr.pos_head, CATEGORY_HEAD);

        // Rotate left 180 around Y axis, so that it's near right
        var avg = GetAvergePoints_180(log, vr.pos_left, vr.pos_right, vr.pos_head);

        // Calculate direction facing
        var forward = GetDirectionFacing(log, avg.pos_left, avg.pos_right, vr);

        return new Points_Pass()
        {
            pos_left = avg.pos_left,
            pos_right = avg.pos_right,
            center_point = forward.center_point,
            forward = forward.forward,
        };
    }
    /// <summary>
    /// This mirrors the left and right controllers to the other side, based on the midpoint and forward direction
    /// calculated in the first pass.  Takes the average of (left, mirrored right) and average of (right, mirroed left)
    /// </summary>
    private Points_Pass GetPoints_SecondPass(DebugLogger log, Points_VR vr, Points_Pass pass1)
    {
        log.Add_Dot(vr.pos_left, CATEGORY_LEFT);
        log.Add_Dot(vr.pos_right, CATEGORY_RIGHT);
        log.Add_Dot(vr.pos_head, CATEGORY_HEAD);

        log.Add_Dot(pass1.center_point, CATEGORY_PASS);
        log.Add_Line(pass1.center_point, pass1.center_point + pass1.forward, CATEGORY_PASS);

        // Mirror left over to right, right over to left
        var avg = GetAvergePoints_Mirror(log, vr.pos_left, vr.pos_right, pass1.center_point, pass1.forward);

        // Calculate direction facing
        var forward = GetDirectionFacing(log, avg.pos_left, avg.pos_right, vr);

        return new Points_Pass()
        {
            pos_left = avg.pos_left,
            pos_right = avg.pos_right,
            center_point = forward.center_point,
            forward = forward.forward,
        };
    }

    private static (Vector3 pos_left, Vector3 pos_right) GetAvergePoints_180(DebugLogger log, Vector3 left, Vector3 right, Vector3 center)
    {
        // Get the positions relative to center
        Vector3 dir_left = left - center;
        Vector3 dir_right = right - center;

        // Rotate 180 so that left_180 is near right
        Vector3 dir_left_rotated = _quat180_y * dir_left;
        Vector3 dir_right_rotated = _quat180_y * dir_right;

        log.Add_Line(center, center + dir_left);
        log.Add_Line(center, center + dir_right);

        log.Add_Line(center, center + dir_left_rotated, CATEGORY_LEFT);
        log.Add_Line(center, center + dir_right_rotated, CATEGORY_RIGHT);

        // Take the averages of (left to right rotated) and (right to left rotated)
        Vector3 dir_left_avg = (dir_left + dir_right_rotated) / 2;
        Vector3 dir_right_avg = (dir_right + dir_left_rotated) / 2;

        log.Add_Dot(center + dir_left_avg);
        log.Add_Dot(center + dir_right_avg);

        // Rotate those averages around
        Vector3 dir_left_avg_rotated = _quat180_y * dir_left_avg;
        Vector3 dir_right_avg_rotated = _quat180_y * dir_right_avg;

        log.Add_Dot(center + dir_left_avg_rotated);
        log.Add_Dot(center + dir_right_avg_rotated);

        // Take the average of the averages.  This is a final stable point for left and right
        Vector3 dir_left_dblavg = (dir_left_avg + dir_right_avg_rotated) / 2;
        Vector3 dir_right_dblavg = (dir_right_avg + dir_left_avg_rotated) / 2;

        Vector3 retval_left = center + dir_left_dblavg;
        Vector3 retval_right = center + dir_right_dblavg;

        log.Add_Dot(retval_left);
        log.Add_Dot(retval_right);

        return (retval_left, retval_right);
    }
    private static (Vector3 pos_left, Vector3 pos_right) GetAvergePoints_Mirror(DebugLogger log, Vector3 left, Vector3 right, Vector3 center, Vector3 forward)
    {
        // Get the positions relative to center
        Vector3 dir_left = left - center;
        Vector3 dir_right = right - center;

        // Project to plane (need to do this so that the rotation will be in the XZ plane)
        Vector3 left_projectplane = Vector3.ProjectOnPlane(dir_left, Vector3.up);
        Vector3 right_projectplane = Vector3.ProjectOnPlane(dir_right, Vector3.up);

        // Get quat for projected to forward
        Quaternion rot_left = Quaternion.FromToRotation(left_projectplane, forward);
        Quaternion rot_right = Quaternion.FromToRotation(right_projectplane, forward);

        // Rotate the actual by that quat * 2
        Vector3 dir_left_rotated = rot_left * (rot_left * dir_left);
        Vector3 dir_right_rotated = rot_right * (rot_right * dir_right);

        log.Add_Line(center, center + dir_left_rotated, CATEGORY_LEFT);
        log.Add_Line(center, center + dir_right_rotated, CATEGORY_RIGHT);

        // Take the averages of (left to right rotated) and (right to left rotated)
        Vector3 dir_left_avg = (dir_left + dir_right_rotated) / 2;
        Vector3 dir_right_avg = (dir_right + dir_left_rotated) / 2;

        log.Add_Dot(center + dir_left_avg);
        log.Add_Dot(center + dir_right_avg);

        //NOTE: The 180 function did a double averaging, That's not needed here, since the points are mirrored

        Vector3 retval_left = center + dir_left_avg;
        Vector3 retval_right = center + dir_right_avg;

        log.Add_Dot(retval_left);
        log.Add_Dot(retval_right);

        return (retval_left, retval_right);
    }

    private static (Vector3 center_point, Vector3 forward) GetDirectionFacing(DebugLogger log, Vector3 left, Vector3 right, Points_VR vr)
    {
        log.Add_Line(left, right);

        Vector3 line = (right - left).normalized;

        Vector3 head_forward_xz = Vector3.ProjectOnPlane(vr.head_forward, Vector3.up).normalized;

        Vector3 cross_up = Vector3.Cross(head_forward_xz, line);

        Vector3 forward = Vector3.Cross(cross_up, line);
        if (Vector3.Dot(head_forward_xz, forward) < 0)
            forward = -forward;

        Vector3 line_mid = left + ((right - left) / 2);

        log.Add_Line(line_mid, line_mid + head_forward_xz, CATEGORY_HEAD);
        log.Add_Line(line_mid, line_mid + forward);

        return (line_mid, forward);
    }

    private static Points_Pass AdjustCenter(Vector3 p1, Vector3 p2, Points_Pass pass)
    {
        Vector3 offset = ((p1 + p2) / 2f) - pass.center_point;

        return new Points_Pass()
        {
            pos_left = pass.pos_left + offset,
            pos_right = pass.pos_right + offset,
            center_point = pass.center_point + offset,
            forward = pass.forward,
        };
    }
}

using PerfectlyNormalUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HUDFlight : MonoBehaviour
{
    #region class: TrackedItem

    private class TrackedItem
    {
        public DebugItem DebugItem { get; set; }

        // These are used to return the current world position that the item should be rendered at

        // Populated when it's a point (not a line)
        public Func<Dictionary<string, object>, Vector3> GetWorld_Position { get; set; }

        // Populated when it's a line
        public Func<Dictionary<string, object>, Vector3> GetWorld_LineFrom { get; set; }
        public Func<Dictionary<string, object>, Vector3> GetWorld_LineTo { get; set; }

        // Populated when it's a circle
        public Func<Dictionary<string, object>, Vector3> GetWorld_Center { get; set; }
        public Func<Dictionary<string, object>, Vector3> GetWorld_Up { get; set; }
    }

    #endregion

    private const float DOT_SIZE_MAIN = 0.01f;
    private const float LINE_THICKNESS = 0.003f;
    private const float RADIUS_ZERO = 0.1f;
    private const float FORWARD_SCALE = 0.5f;
    private const float RADIUS_ROTATION = 0.04f;        // if this is much smaller, the forward circle will only draw as an arc (bug with the line)

    public FlightStartStopTracker StartStopTracker;
    public FlightControllerValues FlightControllerValues;

    public Transform XROrigin;
    public Transform BodyCollider;
    public Transform Foot;
    public Transform HeadController;
    public Transform LeftController;
    public Transform RightController;

    private DebugRenderer3D _renderer = null;

    private List<TrackedItem> _items = new List<TrackedItem>();

    /// <summary>
    /// This is cleared at the beginning of update, then passed to all the functions.  It can be used to hold the result
    /// of expensive calculations
    /// </summary>
    private Dictionary<string, object> _update_props = new Dictionary<string, object>();

    private Color _color_zero = Color.white; //UtilityUnity.ColorFromHex("40FFFFFF");        //TODO: figure out why transparency isn't working
    private Color _color_hand = Color.black;

    private DebugLogger _log;
    private DateTime _last_log_save = DateTime.UtcNow;

    //TODO: ShowSpeed (canvas overlay may need to be handled in a different class)

    private bool _showVisuals_1_1 = false;
    public bool ShowVisuals_1_1
    {
        get => _showVisuals_1_1;
        set
        {
            if (value == _showVisuals_1_1)
                return;

            _showVisuals_1_1 = value;

            if (StartStopTracker.IsInFlight)
            {
                EnsureVisuals_Removed();
                EnsureVisuals_Created();
            }
        }
    }

    private bool _showVisuals_InFront = false;
    public bool ShowVisuals_InFront
    {
        get => _showVisuals_InFront;
        set
        {
            if (value == _showVisuals_InFront)
                return;

            _showVisuals_InFront = value;

            if (StartStopTracker.IsInFlight)
            {
                EnsureVisuals_Removed();
                EnsureVisuals_Created();
            }
        }
    }

    private void Start()
    {
        _renderer = gameObject.AddComponent<DebugRenderer3D>();
    }

    private void Update()
    {
        if (!StartStopTracker.IsInFlight)
        {
            EnsureVisuals_Removed();
            return;
        }

        EnsureVisuals_Created();

        _update_props.Clear();

        foreach (TrackedItem item in _items)
        {
            if (item.GetWorld_Position != null)
            {
                item.DebugItem.Object.transform.position = item.GetWorld_Position(_update_props);
            }
            else if (item.GetWorld_LineFrom != null)
            {
                DebugRenderer3D.AdjustLinePositions(item.DebugItem, item.GetWorld_LineFrom(_update_props), item.GetWorld_LineTo(_update_props));
            }
            else if (item.GetWorld_Center != null)
            {
                item.DebugItem.Object.transform.position = item.GetWorld_Center(_update_props);
                item.DebugItem.Object.transform.rotation = Quaternion.FromToRotation(Vector3.up, item.GetWorld_Up(_update_props));
            }
        }

        //DateTime now = DateTime.UtcNow;
        //if ((now - _last_log_save).TotalSeconds > 1)
        //{
        //    LogForward();
        //    _last_log_save = now;
        //}
    }

    private void EnsureVisuals_Created()
    {
        if (_items.Count > 0)
            return;

        if (!_showVisuals_1_1 && !_showVisuals_InFront)
            return;

        BuildVisuals_Standard();

        // TODO: decide what to draw based on flight mode
        BuildVisuals_Disc();
    }
    private void EnsureVisuals_Removed()
    {
        if (_items.Count == 0)
            return;

        _renderer.Remove(_items.Select(o => o.DebugItem));
        _items.Clear();
    }

    private void BuildVisuals_Standard()
    {
        // Zero Left
        if (_showVisuals_1_1)
            _items.Add(new TrackedItem()
            {
                GetWorld_Position = new Func<Dictionary<string, object>, Vector3>(props => XROrigin.TransformPoint(StartStopTracker.Center_Left)),
                DebugItem = _renderer.AddDot(new Vector3(), DOT_SIZE_MAIN, _color_zero),
            });

        if (_showVisuals_InFront)
            _items.Add(new TrackedItem()
            {
                GetWorld_Position = new Func<Dictionary<string, object>, Vector3>(props => GetForwardPos(StartStopTracker.Center_Left, props)),
                DebugItem = _renderer.AddDot(new Vector3(), DOT_SIZE_MAIN * FORWARD_SCALE, _color_zero),
            });

        // Zero Right
        if (_showVisuals_1_1)
            _items.Add(new TrackedItem()
            {
                GetWorld_Position = new Func<Dictionary<string, object>, Vector3>(props => XROrigin.TransformPoint(StartStopTracker.Center_Right)),
                DebugItem = _renderer.AddDot(new Vector3(), DOT_SIZE_MAIN, _color_zero),
            });

        if (_showVisuals_InFront)
            _items.Add(new TrackedItem()
            {
                GetWorld_Position = new Func<Dictionary<string, object>, Vector3>(props => GetForwardPos(StartStopTracker.Center_Right, props)),
                DebugItem = _renderer.AddDot(new Vector3(), DOT_SIZE_MAIN * FORWARD_SCALE, _color_zero),
            });

        // Actual Left
        if (_showVisuals_1_1)
            _items.Add(new TrackedItem()
            {
                GetWorld_Position = new Func<Dictionary<string, object>, Vector3>(props => LeftController.position),
                DebugItem = _renderer.AddDot(new Vector3(), DOT_SIZE_MAIN, _color_hand),
            });

        if (_showVisuals_InFront)
            _items.Add(new TrackedItem()
            {
                GetWorld_Position = new Func<Dictionary<string, object>, Vector3>(props => GetForwardPos(XROrigin.InverseTransformPoint(LeftController.position), props)),
                DebugItem = _renderer.AddDot(new Vector3(), DOT_SIZE_MAIN * FORWARD_SCALE, _color_hand),
            });

        // Actual Right
        if (_showVisuals_1_1)
            _items.Add(new TrackedItem()
            {
                GetWorld_Position = new Func<Dictionary<string, object>, Vector3>(props => RightController.position),
                DebugItem = _renderer.AddDot(new Vector3(), DOT_SIZE_MAIN, _color_hand),
            });

        if (_showVisuals_InFront)
            _items.Add(new TrackedItem()
            {
                GetWorld_Position = new Func<Dictionary<string, object>, Vector3>(props => GetForwardPos(XROrigin.InverseTransformPoint(RightController.position), props)),
                DebugItem = _renderer.AddDot(new Vector3(), DOT_SIZE_MAIN * FORWARD_SCALE, _color_hand),
            });

        // Rotation Left
        if (_showVisuals_1_1)
            _items.Add(new TrackedItem()
            {
                GetWorld_Center = new Func<Dictionary<string, object>, Vector3>(props => LeftController.position),
                GetWorld_Up = new Func<Dictionary<string, object>, Vector3>(props => XROrigin.TransformDirection(FlightControllerValues.Up_Left)),
                DebugItem = _renderer.AddCircle(new Vector3(), Vector3.up, RADIUS_ROTATION, LINE_THICKNESS, _color_hand),
            });

        if (_showVisuals_InFront)
            _items.Add(new TrackedItem()
            {
                GetWorld_Center = new Func<Dictionary<string, object>, Vector3>(props => GetForwardPos(XROrigin.InverseTransformPoint(LeftController.position), props)),
                GetWorld_Up = new Func<Dictionary<string, object>, Vector3>(props => XROrigin.TransformDirection(FlightControllerValues.Up_Left)),
                DebugItem = _renderer.AddCircle(new Vector3(), Vector3.up, RADIUS_ROTATION * FORWARD_SCALE, LINE_THICKNESS * FORWARD_SCALE, _color_hand),
            });

        // Rotation Right
        if (_showVisuals_1_1)
            _items.Add(new TrackedItem()
            {
                GetWorld_Center = new Func<Dictionary<string, object>, Vector3>(props => RightController.position),
                GetWorld_Up = new Func<Dictionary<string, object>, Vector3>(props => XROrigin.TransformDirection(FlightControllerValues.Up_Right)),
                DebugItem = _renderer.AddCircle(new Vector3(), Vector3.up, RADIUS_ROTATION, LINE_THICKNESS, _color_hand),
            });

        if (_showVisuals_InFront)
            _items.Add(new TrackedItem()
            {
                GetWorld_Center = new Func<Dictionary<string, object>, Vector3>(props => GetForwardPos(XROrigin.InverseTransformPoint(RightController.position), props)),
                GetWorld_Up = new Func<Dictionary<string, object>, Vector3>(props => XROrigin.TransformDirection(FlightControllerValues.Up_Right)),
                DebugItem = _renderer.AddCircle(new Vector3(), Vector3.up, RADIUS_ROTATION * FORWARD_SCALE, LINE_THICKNESS * FORWARD_SCALE, _color_hand),
            });

        // Middle Line
        //_items.Add(new TrackedItem()
        //{
        //    GetWorld_LineFrom = new Func<Dictionary<string, object>, Vector3>(props => (XROrigin.TransformPoint(StartStopTracker.Center_Right) + XROrigin.TransformPoint(StartStopTracker.Center_Left)) / 2f),
        //    GetWorld_LineTo = new Func<Dictionary<string, object>, Vector3>(props => ((XROrigin.TransformPoint(StartStopTracker.Center_Right) + XROrigin.TransformPoint(StartStopTracker.Center_Left)) / 2f) + XROrigin.TransformDirection(StartStopTracker.Forward)),
        //    DebugItem = _renderer.AddLine_Basic(new Vector3(), new Vector3(), LINE_THICKNESS, color_zero),
        //});
    }
    private void BuildVisuals_Disc()
    {
        // Circle Left
        if (_showVisuals_1_1)
            _items.Add(new TrackedItem()
            {
                GetWorld_Center = new Func<Dictionary<string, object>, Vector3>(props => XROrigin.TransformPoint(StartStopTracker.Center_Left)),
                GetWorld_Up = new Func<Dictionary<string, object>, Vector3>(props => XROrigin.up),
                DebugItem = _renderer.AddCircle(new Vector3(), Vector3.up, RADIUS_ZERO, LINE_THICKNESS, _color_zero),
            });

        if (_showVisuals_InFront)
            _items.Add(new TrackedItem()
            {
                GetWorld_Center = new Func<Dictionary<string, object>, Vector3>(props => GetForwardPos(StartStopTracker.Center_Left, props)),
                GetWorld_Up = new Func<Dictionary<string, object>, Vector3>(props => XROrigin.up),
                DebugItem = _renderer.AddCircle(new Vector3(), Vector3.up, RADIUS_ZERO * FORWARD_SCALE, LINE_THICKNESS * FORWARD_SCALE, _color_zero),
            });

        // Circle Right
        if (_showVisuals_1_1)
            _items.Add(new TrackedItem()
            {
                GetWorld_Center = new Func<Dictionary<string, object>, Vector3>(props => XROrigin.TransformPoint(StartStopTracker.Center_Right)),
                GetWorld_Up = new Func<Dictionary<string, object>, Vector3>(props => XROrigin.up),
                DebugItem = _renderer.AddCircle(new Vector3(), Vector3.up, RADIUS_ZERO, LINE_THICKNESS, _color_zero),
            });

        if (_showVisuals_InFront)
            _items.Add(new TrackedItem()
            {
                GetWorld_Center = new Func<Dictionary<string, object>, Vector3>(props => GetForwardPos(StartStopTracker.Center_Right, props)),
                GetWorld_Up = new Func<Dictionary<string, object>, Vector3>(props => XROrigin.up),
                DebugItem = _renderer.AddCircle(new Vector3(), Vector3.up, RADIUS_ZERO * FORWARD_SCALE, LINE_THICKNESS * FORWARD_SCALE, _color_zero),
            });

        // Plane Line Left
        if (_showVisuals_1_1)
            _items.Add(new TrackedItem()
            {
                GetWorld_LineFrom = new Func<Dictionary<string, object>, Vector3>(props => XROrigin.TransformPoint(StartStopTracker.Center_Left)),
                GetWorld_LineTo = new Func<Dictionary<string, object>, Vector3>(props => XROrigin.TransformPoint(FlightControllerValues.Plane_Left)),
                DebugItem = _renderer.AddLine_Basic(new Vector3(), new Vector3(), LINE_THICKNESS, _color_hand),
            });

        if (_showVisuals_InFront)
            _items.Add(new TrackedItem()
            {
                GetWorld_LineFrom = new Func<Dictionary<string, object>, Vector3>(props => GetForwardPos(StartStopTracker.Center_Left, props)),
                GetWorld_LineTo = new Func<Dictionary<string, object>, Vector3>(props => GetForwardPos(FlightControllerValues.Plane_Left, props)),
                DebugItem = _renderer.AddLine_Basic(new Vector3(), new Vector3(), LINE_THICKNESS, _color_hand),
            });

        // Plane Line Right
        if (_showVisuals_1_1)
            _items.Add(new TrackedItem()
            {
                GetWorld_LineFrom = new Func<Dictionary<string, object>, Vector3>(props => XROrigin.TransformPoint(StartStopTracker.Center_Right)),
                GetWorld_LineTo = new Func<Dictionary<string, object>, Vector3>(props => XROrigin.TransformPoint(FlightControllerValues.Plane_Right)),
                DebugItem = _renderer.AddLine_Basic(new Vector3(), new Vector3(), LINE_THICKNESS, _color_hand),
            });

        if (_showVisuals_InFront)
            _items.Add(new TrackedItem()
            {
                GetWorld_LineFrom = new Func<Dictionary<string, object>, Vector3>(props => GetForwardPos(StartStopTracker.Center_Right, props)),
                GetWorld_LineTo = new Func<Dictionary<string, object>, Vector3>(props => GetForwardPos(FlightControllerValues.Plane_Right, props)),
                DebugItem = _renderer.AddLine_Basic(new Vector3(), new Vector3(), LINE_THICKNESS, _color_hand),
            });

        // Vertical Line Left
        if (_showVisuals_1_1)
            _items.Add(new TrackedItem()
            {
                GetWorld_LineFrom = new Func<Dictionary<string, object>, Vector3>(props => XROrigin.TransformPoint(FlightControllerValues.Plane_Left)),
                GetWorld_LineTo = new Func<Dictionary<string, object>, Vector3>(props => LeftController.position),
                DebugItem = _renderer.AddLine_Basic(new Vector3(), new Vector3(), LINE_THICKNESS, _color_hand),
            });

        if (_showVisuals_InFront)
            _items.Add(new TrackedItem()
            {
                GetWorld_LineFrom = new Func<Dictionary<string, object>, Vector3>(props => GetForwardPos(FlightControllerValues.Plane_Left, props)),
                GetWorld_LineTo = new Func<Dictionary<string, object>, Vector3>(props => GetForwardPos(XROrigin.InverseTransformPoint(LeftController.position), props)),
                DebugItem = _renderer.AddLine_Basic(new Vector3(), new Vector3(), LINE_THICKNESS, _color_hand),
            });

        // Vertical Line Right
        if (_showVisuals_1_1)
            _items.Add(new TrackedItem()
            {
                GetWorld_LineFrom = new Func<Dictionary<string, object>, Vector3>(props => XROrigin.TransformPoint(FlightControllerValues.Plane_Right)),
                GetWorld_LineTo = new Func<Dictionary<string, object>, Vector3>(props => RightController.position),
                DebugItem = _renderer.AddLine_Basic(new Vector3(), new Vector3(), LINE_THICKNESS, _color_hand),
            });

        if (_showVisuals_InFront)
            _items.Add(new TrackedItem()
            {
                GetWorld_LineFrom = new Func<Dictionary<string, object>, Vector3>(props => GetForwardPos(FlightControllerValues.Plane_Right, props)),
                GetWorld_LineTo = new Func<Dictionary<string, object>, Vector3>(props => GetForwardPos(XROrigin.InverseTransformPoint(RightController.position), props)),
                DebugItem = _renderer.AddLine_Basic(new Vector3(), new Vector3(), LINE_THICKNESS, _color_hand),
            });
    }

    private Vector3 GetForwardPos(Vector3 pos_local, Dictionary<string, object> props)
    {
        const float FORWARD_SIZE = 0.25f;
        const float FORWARD_OFFSET = 0.5f;
        const float UP_OFFSET = 0.35f;

        Vector3 pos_world = XROrigin.transform.TransformPoint(pos_local);

        Vector3 center = XROrigin.transform.TransformPoint(StartStopTracker.MidPoint);

        Vector3 offset = pos_world - center;

        offset *= FORWARD_SIZE;

        pos_world = center + offset;

        pos_world += HeadController.forward * FORWARD_OFFSET;
        pos_world += XROrigin.up * UP_OFFSET;

        return pos_world;
    }

    private void LogForward()
    {
        const string NEUTRAL = "neutral";
        const string LEFT = "left";
        const string RIGHT = "right";
        const string ORIGIN = "xr origin";
        const string PLAYER = "player";

        if (_log == null)
        {
            _log = new DebugLogger(@"D:\temp\hud forward", true);

            _log.DefineCategory(NEUTRAL, Color.gray);
            _log.DefineCategory(LEFT, Color.red);
            _log.DefineCategory(RIGHT, Color.green);
            _log.DefineCategory(ORIGIN, Color.cyan);
            _log.DefineCategory(PLAYER, Color.yellow);
        }

        _log.NewFrame("floor coords");

        _log.Add_Dot(StartStopTracker.Center_Left, LEFT);
        _log.Add_Circle(StartStopTracker.Center_Left, Vector3.up, 0.2f, LEFT);

        _log.Add_Dot(StartStopTracker.Center_Right, RIGHT);
        _log.Add_Circle(StartStopTracker.Center_Right, Vector3.up, 0.2f, RIGHT);

        Vector3 mid = StartStopTracker.Center_Left + ((StartStopTracker.Center_Right - StartStopTracker.Center_Left) / 2);
        _log.Add_Line(mid, mid + StartStopTracker.Forward, NEUTRAL);

        _log.Add_Dot(XROrigin.InverseTransformPoint(XROrigin.position), ORIGIN);

        _log.Add_Dot(XROrigin.InverseTransformPoint(BodyCollider.position), PLAYER);
        _log.Add_Dot(XROrigin.InverseTransformPoint(HeadController.position), PLAYER);


        _log.NewFrame("world coords - origin");

        _log.Add_Dot(XROrigin.TransformPoint(StartStopTracker.Center_Left), LEFT);
        _log.Add_Circle(XROrigin.TransformPoint(StartStopTracker.Center_Left), XROrigin.up, 0.2f, LEFT);

        _log.Add_Dot(XROrigin.TransformPoint(StartStopTracker.Center_Right), RIGHT);
        _log.Add_Circle(XROrigin.TransformPoint(StartStopTracker.Center_Right), XROrigin.up, 0.2f, RIGHT);

        mid = XROrigin.TransformPoint(StartStopTracker.Center_Left + ((StartStopTracker.Center_Right - StartStopTracker.Center_Left) / 2));
        _log.Add_Line(mid, mid + XROrigin.TransformDirection(StartStopTracker.Forward), NEUTRAL);

        _log.Add_Dot(XROrigin.position, ORIGIN);

        _log.Add_Dot(BodyCollider.position, PLAYER);
        _log.Add_Dot(HeadController.position, PLAYER);


        _log.NewFrame("world coords - head");

        _log.Add_Dot(HeadController.TransformPoint(StartStopTracker.Center_Left), LEFT);
        _log.Add_Circle(HeadController.TransformPoint(StartStopTracker.Center_Left), HeadController.up, 0.2f, LEFT);

        _log.Add_Dot(HeadController.TransformPoint(StartStopTracker.Center_Right), RIGHT);
        _log.Add_Circle(HeadController.TransformPoint(StartStopTracker.Center_Right), HeadController.up, 0.2f, RIGHT);

        mid = HeadController.TransformPoint(StartStopTracker.Center_Left + ((StartStopTracker.Center_Right - StartStopTracker.Center_Left) / 2));
        _log.Add_Line(mid, mid + HeadController.TransformDirection(StartStopTracker.Forward), NEUTRAL);

        _log.Add_Dot(XROrigin.position, ORIGIN);

        _log.Add_Dot(BodyCollider.position, PLAYER);
        _log.Add_Dot(HeadController.position, PLAYER);


        _log.Save();
    }

    #region ATTEMPTS

    // I give up trying to rotate into head's orientation relative to xr origin's orientation

    //private Vector3 GetForwardPos_ORIG(Vector3 pos_local, Dictionary<string, object> props)
    //{
    //    const float FORWARD_SCALE = 0.25f;
    //    const float FORWARD_OFFSET = 0.8f;
    //    const string PROP_QUAT = "GetForwardPos_quat";

    //    // Place it in front of the camera
    //    Vector3 forward_pos = HeadController.position + HeadController.forward * FORWARD_OFFSET;

    //    // TODO: push down a little (along -head.up)

    //    // Get relative to head (and scale down)
    //    Vector3 pos_world = XROrigin.TransformPoint(pos_local);

    //    Vector3 dir = (pos_world - HeadController.position) * FORWARD_SCALE;

    //    // Rotate dir into the plane where forward is the normal
    //    Quaternion quat;
    //    if (props.TryGetValue(PROP_QUAT, out object quat_obj))
    //    {
    //        quat = (Quaternion)quat_obj;
    //    }
    //    else
    //    {

    //        //TODO: rotating from model coords to the final, but points passed in are in world coords
    //        //Need a transform from world to model first
    //        //Or figure out how to go straight from world to in front world

    //        quat = Math3D.GetRotation(new DoubleVector(StartStopTracker.Forward, Vector3.up), new DoubleVector(HeadController.forward, HeadController.up));
    //    }

    //    dir = quat * dir;

    //    return forward_pos + dir;
    //}
    //private Vector3 GetForwardPos_CLOSER(Vector3 pos_local, Dictionary<string, object> props)
    //{
    //    const float SCALE = 0.25f;
    //    const float FORWARD_OFFSET = 0.8f;
    //    const float DOWN_OFFSET = 0.4f;

    //    pos_local = pos_local * SCALE;

    //    //TODO: this isn't taking the player's position relative to xr origin into account
    //    Vector3 pos_world = HeadController.transform.TransformPoint(pos_local);

    //    pos_world += HeadController.forward * FORWARD_OFFSET;
    //    pos_world += HeadController.up * -DOWN_OFFSET;

    //    return pos_world;
    //}
    //private Vector3 GetForwardPos_PLANEOFFSET(Vector3 pos_local, Dictionary<string, object> props)
    //{
    //    const float SCALE = 0.25f;
    //    const float FORWARD_OFFSET = 0.8f;
    //    const float DOWN_OFFSET = 0.4f;

    //    pos_local = pos_local * SCALE;

    //    Vector3 pos_world = HeadController.transform.TransformPoint(pos_local);

    //    // need to take the player's position relative to xr origin into account
    //    Vector3 head_on_plane = Math3D.GetClosestPoint_Plane_Point(new Plane(XROrigin.up, XROrigin.position), HeadController.position);

    //    //Vector3 room_offset = XROrigin.position - head_on_plane;      // too far forward left
    //    Vector3 room_offset = head_on_plane - XROrigin.position;        // behind and to the right of the head
    //    //pos_world += room_offset;

    //    pos_world += HeadController.forward * FORWARD_OFFSET;
    //    //pos_world += HeadController.up * -DOWN_OFFSET;

    //    return pos_world;
    //}
    //private Vector3 GetForwardPos_ROOMOFFSET_POST(Vector3 pos_local, Dictionary<string, object> props)
    //{
    //    const float SCALE = 0.25f;
    //    const float FORWARD_OFFSET = 0.8f;
    //    const float DOWN_OFFSET = 0.4f;

    //    pos_local = pos_local * SCALE;

    //    Vector3 pos_world = HeadController.transform.TransformPoint(pos_local);

    //    Vector3 room_offset = XROrigin.position - HeadController.position;
    //    //pos_world += room_offset;

    //    pos_world += HeadController.forward * FORWARD_OFFSET;
    //    //pos_world += HeadController.up * -DOWN_OFFSET;

    //    return pos_world;
    //}
    //private Vector3 GetForwardPos_ROOMOFFSET_PRE(Vector3 pos_local, Dictionary<string, object> props)
    //{
    //    const float SCALE = 0.25f;
    //    const float FORWARD_OFFSET = 0.8f;
    //    const float DOWN_OFFSET = 0.4f;

    //    pos_local = pos_local * SCALE;

    //    Vector3 inv_xrorigin = XROrigin.InverseTransformPoint(Vector3.zero);
    //    Vector3 inv_collider = BodyCollider.InverseTransformPoint(Vector3.zero);
    //    Vector3 offset_room = new Vector3(inv_collider.x - inv_xrorigin.x, 0, inv_collider.z - inv_xrorigin.z);
    //    //Vector3 offset_room = new Vector3(inv_xrorigin.x - inv_collider.x, 0, inv_xrorigin.z - inv_collider.z);

    //    pos_local += offset_room;
    //    //pos_local -= offset_room;     // worse

    //    Vector3 pos_world = HeadController.transform.TransformPoint(pos_local);

    //    pos_world += HeadController.forward * FORWARD_OFFSET;
    //    pos_world += HeadController.up * -DOWN_OFFSET;

    //    return pos_world;
    //}
    //private Vector3 GetForwardPos_POSTROTATE(Vector3 pos_local, Dictionary<string, object> props)
    //{
    //    const float FORWARD_SCALE = 0.25f;
    //    const float FORWARD_OFFSET = 0.8f;

    //    Vector3 pos_world = XROrigin.transform.TransformPoint(pos_local);


    //    // this doesn't work
    //    Quaternion quat = Math3D.GetRotation(XROrigin.rotation, HeadController.rotation);
    //    pos_world = quat * pos_world;


    //    return pos_world;
    //}
    //private Vector3 GetForwardPos_I_DONT_KNOW(Vector3 pos_local, Dictionary<string, object> props)
    //{
    //    const float SCALE = 0.25f;
    //    const float FORWARD_OFFSET = 0.8f;
    //    const float DOWN_OFFSET = 0.4f;

    //    //pos_local = pos_local * SCALE;

    //    Vector3 inv_collider = XROrigin.InverseTransformPoint(BodyCollider.position);
    //    Vector3 offset_room = new Vector3(inv_collider.x, 0, inv_collider.z);

    //    pos_local += offset_room;

    //    Vector3 pos_world = HeadController.transform.TransformPoint(pos_local);

    //    //pos_world += HeadController.forward * FORWARD_OFFSET;
    //    //pos_world += HeadController.up * -DOWN_OFFSET;

    //    return pos_world;
    //}
    //private Vector3 GetForwardPos_POSTROTATE_AROUNDWORLDCENTER(Vector3 pos_local, Dictionary<string, object> props)
    //{
    //    const float FORWARD_SCALE = 0.25f;
    //    const float FORWARD_OFFSET = 0.4f;
    //    const float UP_OFFSET = 0.3f;


    //    // I don't trust this function
    //    Quaternion quat = Math3D.GetRotation(XROrigin.rotation, HeadController.rotation);



    //    Vector3 pos_world = XROrigin.transform.TransformPoint(pos_local);

    //    Vector3 center = XROrigin.transform.TransformPoint(StartStopTracker.MidPoint);

    //    Vector3 offset = pos_world - center;
    //    offset = quat * offset;

    //    offset *= FORWARD_SCALE;

    //    pos_world = center + offset;

    //    //pos_world += HeadController.forward * FORWARD_OFFSET;
    //    //pos_world += XROrigin.up * UP_OFFSET;

    //    return pos_world;
    //}

    #endregion
}

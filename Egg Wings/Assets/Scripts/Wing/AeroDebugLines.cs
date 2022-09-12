using PerfectlyNormalUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: lines for span, chord, forward, up (to make sure the rotations didn't mess something up)

public class AeroDebugLines : MonoBehaviour
{
    private const float DOT_SIZE_MAIN = 0.02f;
    private const float LINE_THICKNESS = 0.01f;

    //public Vector3 Force_Deflect = Vector3.zero;
    //private DebugItem _deflect = null;

    public Vector3 Force_Lift = Vector3.zero;
    private DebugItem _lift = null;

    public Vector3 Force_Drag = Vector3.zero;
    private DebugItem _drag = null;

    private DebugItem _span = null;
    private DebugItem _chord = null;
    private DebugItem _forward = null;
    private DebugItem _up = null;

    private DebugRenderer3D _renderer = null;

    private Rigidbody _body = null;

    void Start()
    {
        _body = GetComponentInParent<Rigidbody>();      // this is recursive

        _renderer = gameObject.AddComponent<DebugRenderer3D>();
        //_deflect = _renderer.AddLine_Basic(Vector3.zero, Vector3.zero, LINE_THICKNESS, UtilityUnity.ColorFromHex("6C11CF"));
        _lift = _renderer.AddLine_Basic(Vector3.zero, Vector3.zero, LINE_THICKNESS, UtilityUnity.ColorFromHex("E3D029"));
        _drag = _renderer.AddLine_Basic(Vector3.zero, Vector3.zero, LINE_THICKNESS, UtilityUnity.ColorFromHex("C41D58"));

        _span = _renderer.AddLine_Basic(Vector3.zero, Vector3.zero, LINE_THICKNESS, UtilityUnity.ColorFromHex("800"));
        _chord = _renderer.AddLine_Basic(Vector3.zero, Vector3.zero, LINE_THICKNESS, UtilityUnity.ColorFromHex("008"));

        _forward = _renderer.AddLine_Basic(Vector3.zero, Vector3.zero, LINE_THICKNESS, UtilityUnity.ColorFromHex("88F"));
        _up = _renderer.AddLine_Basic(Vector3.zero, Vector3.zero, LINE_THICKNESS, UtilityUnity.ColorFromHex("8F8"));
    }

    void Update()
    {
        Vector3 pos = transform.position;

        float accel_mult = 1f / _body.mass;     // gives an approximate acceleration, useful so all lines will be a normalized length regardless of wing size and mass

        //DebugRenderer3D.AdjustLinePositions(_deflect, pos, pos + (Force_Deflect * accel_mult));
        DebugRenderer3D.AdjustLinePositions(_lift, pos, pos + (Force_Lift * accel_mult));
        DebugRenderer3D.AdjustLinePositions(_drag, pos, pos + (Force_Drag * accel_mult));

        float span_half = transform.lossyScale.x / 2f;
        float chord_half = transform.lossyScale.z / 2f;

        DebugRenderer3D.AdjustLinePositions(_span, pos - (transform.right * span_half), pos + (transform.right * span_half));
        DebugRenderer3D.AdjustLinePositions(_chord, pos - (transform.forward * chord_half), pos + (transform.forward * chord_half));

        DebugRenderer3D.AdjustLinePositions(_forward, pos, pos + transform.forward);
        DebugRenderer3D.AdjustLinePositions(_up, pos, pos + transform.up);
    }
}

using Assets.Scripts.Genetic;
using Assets.Scripts.Genetic.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneBuilder_Attempt1 : MonoBehaviour
{
    public GameObject MountPoint_Left;
    public GameObject MountPoint_Right;
    public GameObject MountPoint_Tail;

    public GameObject Wing_Prefab;

    public Vector3 Offset_Wing = new Vector3(0.25f, 0, 0.5f);
    public Quaternion Rotation_Wing = Quaternion.identity;

    public Vector3 Offset_Tail = new Vector3(0, 0, -0.5f);
    public Quaternion Rotation_Tail = Quaternion.identity;

    // --------------------- Wing ---------------------

    public int WING_Inner_Segment_Count = 2;

    public float WING_Span = 1f;
    public float WING_Span_Power = 1f;       // all inputs to the power function are scaled from 0 to 1.  You generally want to set power between 0.5 and 1

    public float WING_Chord_Base = 0.4f;
    public float WING_Chord_Tip = 0.4f;
    public float WING_Chord_Power = 1f;

    public float WING_Lift_Base = 0.7f;
    public float WING_Lift_Tip = 0.2f;
    public float WING_Lift_Power = 1f;

    public float WING_VerticalStabilizer_Base = 0;       // NOTE: a vertical stabilizer won't be created for a segment if it's less than MIN_VERTICALSTABILIZER_HEIGHT
    public float WING_VerticalStabilizer_Tip = 0;
    public float WING_VerticalStabilizer_Power = 16;

    // --------------------- Tail ---------------------

    public int TAIL_Boom_Inner_Segment_Count = 2;

    public float TAIL_Boom_Length = 1f;
    public float TAIL_Boom_Length_Power = 1f;

    public float TAIL_Boom_Mid_Length = 0.7f;

    public float TAIL_Boom_Span_Base = 0.5f;
    public float TAIL_Boom_Span_Mid = 0.2f;
    public float TAIL_Boom_Span_Tip = 0.3f;

    public float TAIL_Boom_Vert_Base = 0.5f;
    public float TAIL_Boom_Vert_Mid = 0.2f;
    public float TAIL_Boom_Vert_Tip = 0.3f;

    public float TAIL_Boom_Bezier_PinchPercent = 0.2f;       // 0 to 0.4 (affects the curviness of the bezier, 0 is linear)

    public float TAIL_Tail_Chord = 0.33f;
    public float TAIL_Tail_Horz_Span = 0.5f;
    public float TAIL_Tail_Vert_Height = 0.5f;

    public void RebuildPlane()
    {
        PlaneDefinition def = GetDefinition();

        var mountpoints = new PlaneBuilder_MountPoints()
        {
            Tail = MountPoint_Tail,
            Wing_0_Left = MountPoint_Left,
            Wing_0_Right = MountPoint_Right,
        };

        PlaneBuilder.BuildPlane(def, mountpoints, Wing_Prefab);
    }

    private PlaneDefinition GetDefinition()
    {
        return new PlaneDefinition()
        {
            Wing_0 = GetDefinition_Wing(),
            Tail = GetDefinition_Tail(),
        };
    }

    private WingDefinition GetDefinition_Wing()
    {
        return new WingDefinition()
        {
            Offset = Offset_Wing,
            Rotation = Rotation_Wing,

            Inner_Segment_Count = WING_Inner_Segment_Count,

            Span = WING_Span,
            Span_Power = WING_Span_Power,

            Chord_Base = WING_Chord_Base,
            Chord_Tip = WING_Chord_Tip,
            Chord_Power = WING_Chord_Power,

            Lift_Base = WING_Lift_Base,
            Lift_Tip = WING_Lift_Tip,
            Lift_Power = WING_Lift_Power,

            VerticalStabilizer_Base = WING_VerticalStabilizer_Base,
            VerticalStabilizer_Tip = WING_VerticalStabilizer_Tip,
            VerticalStabilizer_Power = WING_VerticalStabilizer_Power,
        };
    }

    private TailDefinition GetDefinition_Tail()
    {
        return new TailDefinition()
        {
            Offset = Offset_Tail,
            Rotation = Rotation_Tail,

            Boom = new TailDefinition_Boom()
            {
                Inner_Segment_Count = TAIL_Boom_Inner_Segment_Count,

                Length = TAIL_Boom_Length,
                Length_Power = TAIL_Boom_Length_Power,

                Mid_Length = TAIL_Boom_Mid_Length,

                Span_Base = TAIL_Boom_Span_Base,
                Span_Mid = TAIL_Boom_Span_Mid,
                Span_Tip = TAIL_Boom_Span_Tip,

                Vert_Base = TAIL_Boom_Vert_Base,
                Vert_Mid = TAIL_Boom_Vert_Mid,
                Vert_Tip = TAIL_Boom_Vert_Tip,

                Bezier_PinchPercent = TAIL_Boom_Bezier_PinchPercent,
            },

            Tail = new TailDefinition_Tail()
            {
                Chord = TAIL_Tail_Chord,
                Horz_Span = TAIL_Tail_Horz_Span,
                Vert_Height = TAIL_Tail_Vert_Height,
            },
        };
    }

}

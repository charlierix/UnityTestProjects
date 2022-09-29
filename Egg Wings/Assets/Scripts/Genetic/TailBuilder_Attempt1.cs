using Assets.Scripts.Genetic;
using Assets.Scripts.Genetic.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TailBuilder_Attempt1 : MonoBehaviour
{
    public GameObject MountPoint;

    public GameObject Wing_Prefab;

    public Vector3 Offset = new Vector3(0, 0, 0);
    public Quaternion Rotation = Quaternion.identity;

    public int Boom_Inner_Segment_Count = 2;

    public float Boom_Length = 1f;
    public float Boom_Length_Power = 1f;

    public float Boom_Mid_Length = 0.7f;

    public float Boom_Span_Base = 0.5f;
    public float Boom_Span_Mid = 0.2f;
    public float Boom_Span_Tip = 0.3f;

    public float Boom_Vert_Base = 0.5f;
    public float Boom_Vert_Mid = 0.2f;
    public float Boom_Vert_Tip = 0.3f;

    public float Boom_Bezier_PinchPercent = 0.2f;       // 0 to 0.4 (affects the curviness of the bezier, 0 is linear)

    public float Tail_Chord = 0.33f;
    public float Tail_Horz_Span = 0.5f;
    public float Tail_Vert_Height = 0.5f;

    public void RebuildTail()
    {
        TailDefinition def = GetDefinition();

        PlaneBuilder.BuildTail(def, MountPoint, Wing_Prefab);
    }

    private TailDefinition GetDefinition()
    {
        return new TailDefinition()
        {
            Offset = Offset,
            Rotation = Rotation,

            Boom = new TailDefinition_Boom()
            {
                Inner_Segment_Count = Boom_Inner_Segment_Count,

                Length = Boom_Length,
                Length_Power = Boom_Length_Power,

                Mid_Length = Boom_Mid_Length,

                Span_Base = Boom_Span_Base,
                Span_Mid = Boom_Span_Mid,
                Span_Tip = Boom_Span_Tip,

                Vert_Base = Boom_Vert_Base,
                Vert_Mid = Boom_Vert_Mid,
                Vert_Tip = Boom_Vert_Tip,

                Bezier_PinchPercent = Boom_Bezier_PinchPercent,
            },

            Tail = new TailDefinition_Tail()
            {
                Chord = Tail_Chord,
                Horz_Span = Tail_Horz_Span,
                Vert_Height = Tail_Vert_Height,
            },
        };
    }
}

using Assets.Scripts.Genetic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WingBuilder_Attempt2 : MonoBehaviour
{
    public GameObject MountPoint_Left;
    public GameObject MountPoint_Right;

    public GameObject Wing_Prefab;

    public int Inner_Segment_Count = 2;

    public float Span = 1f;
    public float Span_Power = 1f;       // all inputs to the power function are scaled from 0 to 1.  You generally want to set power between 0.5 and 1

    public float Chord_Base = 0.4f;
    public float Chord_Tip = 0.4f;
    public float Chord_Power = 1f;

    public float Lift_Base = 0.7f;
    public float Lift_Tip = 0.2f;
    public float Lift_Power = 1f;

    public float VerticalStabilizer_Base = 0;       // NOTE: a vertical stabilizer won't be created for a segment if it's less than MIN_VERTICALSTABILIZER_HEIGHT
    public float VerticalStabilizer_Tip = 0;
    public float VerticalStabilizer_Power = 16;

    public void RebuildWing()
    {
        WingDefinition def = GetDefinition();

        WingBuilder.BuildWing(def, MountPoint_Left, Wing_Prefab, false);
        WingBuilder.BuildWing(def, MountPoint_Right, Wing_Prefab, true);
    }

    private WingDefinition GetDefinition()
    {
        return new WingDefinition()
        {
            Inner_Segment_Count = Inner_Segment_Count,

            Span = Span,
            Span_Power = Span_Power,

            Chord_Base = Chord_Base,
            Chord_Tip = Chord_Tip,
            Chord_Power = Chord_Power,

            Lift_Base = Lift_Base,
            Lift_Tip = Lift_Tip,
            Lift_Power = Lift_Power,

            //MIN_VERTICALSTABILIZER_HEIGHT = ,

            VerticalStabilizer_Base = VerticalStabilizer_Base,
            VerticalStabilizer_Tip = VerticalStabilizer_Tip,
            VerticalStabilizer_Power = VerticalStabilizer_Power,
        };
    }
}

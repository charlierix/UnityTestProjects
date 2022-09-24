using PerfectlyNormalUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class WingBuilder_Attempt1 : MonoBehaviour
{
    private const string UNSCALED = "unscaled";

    public GameObject MountPoint;
    public GameObject Wing_Prefab;

    public float Span = 1f;

    public float Chord_Base = 0.4f;
    public float Chord_Tip = 0.4f;

    public float Power_Span = 1f;
    public float Power_Chord = 1f;

    public int Inner_Segment_Count = 2;

    private BoneRenderer _boneRenderer;

    private void Start()
    {
        _boneRenderer = MountPoint.GetComponent<BoneRenderer>();
    }

    public void RebuildWing()
    {
        Clear();

        Vector3[] points_global = GetPoints(Vector3.zero, new Vector3(Span, 0, 0), Inner_Segment_Count, Power_Span);
        Vector3[] points_relative = ConvertToRelative(points_global);
        float[] chords = GetChords(Chord_Base, Chord_Tip, Power_Chord, points_global, Span);

        // These are the anchor points of each wing segment.  They need to stay unscaled (driven home by the name)
        // These act as parents for each wing segment, and are manipulated by the IK animations
        GameObject[] unscaled = CreateUnscaled(points_relative);

        // Just for visualization, shows the chain that the IK animation manipulates
        PopulateBoneRenderTransforms(unscaled);

        CreateWings(unscaled, chords);



    }

    private void Clear()
    {
        // Clear uscaled (keep Rig)
        foreach (Transform child_transform in MountPoint.transform)
        {
            GameObject child = child_transform.gameObject;

            if (child.name.StartsWith(UNSCALED))
                Destroy(child);
        }

        // Clear bone renderer's transforms
        _boneRenderer.transforms = new Transform[0];
    }

    private GameObject[] CreateUnscaled(Vector3[] points_relative)
    {
        var retVal = new GameObject[points_relative.Length];

        GameObject parent = MountPoint;

        for (int i = 0; i < points_relative.Length; i++)
        {
            GameObject unscaled = new GameObject($"{UNSCALED} {i}");
            unscaled.transform.SetParent(parent.transform, false);

            unscaled.transform.localPosition = points_relative[i];

            parent = unscaled;
            retVal[i] = unscaled;
        }

        return retVal;
    }

    private void PopulateBoneRenderTransforms(GameObject[] unscaled)
    {
        _boneRenderer.transforms = new Transform[unscaled.Length];

        for (int i = 0; i < unscaled.Length; i++)
        {
            _boneRenderer.transforms[i] = unscaled[i].transform;
        }
    }

    private GameObject[] CreateWings(GameObject[] unscaled, float[] chords)
    {
        var retVal = new GameObject[unscaled.Length - 1];        // there is one more unscaled at the end (located where the last wingtip ends)

        for (int i = 0; i < retVal.Length; i++)
        {
            //Vector3 span = unscaled[i + 1].transform.localPosition - unscaled[i].transform.localPosition;
            Vector3 span = unscaled[i + 1].transform.localPosition;     // no need to subtract, because the local position is already the vector from 0 to endpoint

            Vector3 mid_point = span / 2;

            retVal[i] = Instantiate(Wing_Prefab, unscaled[i].transform);

            retVal[i].transform.localPosition = mid_point;

            retVal[i].transform.localScale = new Vector3(
                span.magnitude,
                retVal[i].transform.localScale.y,
                (chords[i] + chords[i + 1]) / 2);
        }

        return retVal;
    }

    private static Vector3[] GetPoints(Vector3 from, Vector3 to, int internal_points, float pow)
    {
        float[] x = GetPoints(from.x, to.x, internal_points, pow);
        float[] y = GetPoints(from.y, to.y, internal_points, pow);
        float[] z = GetPoints(from.z, to.z, internal_points, pow);

        Vector3[] retVal = new Vector3[x.Length];

        for (int i = 0; i < x.Length; i++)
        {
            retVal[i] = new Vector3(x[i], y[i], z[i]);
        }

        return retVal;
    }
    private static float[] GetPoints(float from, float to, int internal_points, float pow)
    {
        float[] retVal = new float[internal_points + 2];

        float step = 1f / (retVal.Length - 1);
        float gap = to - from;

        for (int i = 0; i < retVal.Length; i++)
        {
            float percent = i * step;

            retVal[i] = from + (Mathf.Pow(percent, pow) * gap);
        }

        return retVal;
    }

    private static Vector3[] ConvertToRelative(Vector3[] points_global)
    {
        var retVal = new Vector3[points_global.Length];

        retVal[0] = Vector3.zero;

        for (int i = 0; i < points_global.Length - 1; i++)
        {
            retVal[i + 1] = points_global[i + 1] - points_global[i];
        }

        return retVal;
    }

    private static float[] GetChords(float chord_base, float chord_tip, float power, Vector3[] points_global, float total_span)
    {
        var retVal = new float[points_global.Length];

        retVal[0] = chord_base;
        retVal[^1] = chord_tip;

        for (int i = 1; i < retVal.Length - 1; i++)
        {
            float percent = (points_global[i] - points_global[0]).magnitude / total_span;

            float mult = Mathf.Pow(percent, power);

            retVal[i] = UtilityMath.GetScaledValue(chord_base, chord_tip, 0, 1, mult);
        }

        return retVal;
    }

    private static void NOTES()
    {
        // -------------------- Future Ideas --------------------

        // vertical stabalizers
        //  allow them to be tied to rudder control

        // lift pow
        //  power curve of the lift of each wing segment

        // start/stop angles

        // MULT_WING_ROOT_STRAFE
        // MULT_WING_TIP_STRAFE
        // MULT_WING_TIP_ROLL

    }
}

using PerfectlyNormalUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class WingBuilder_Attempt1 : MonoBehaviour
{
    private const string UNSCALED = "unscaled";
    private const float MIN_VERTICALSTABILIZER_HEIGHT = 0.1f;

    public GameObject MountPoint;
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
    public float VerticalStabilizer_Power = 4;

    private BoneRenderer _boneRenderer;
    private RigBuilder _rigBuilder;
    private TwistChainConstraint[] _twist_constraints;

    private void Start()
    {
        _boneRenderer = MountPoint.GetComponent<BoneRenderer>();
        _rigBuilder = MountPoint.GetComponent<RigBuilder>();
        _twist_constraints = MountPoint.GetComponentsInChildren<TwistChainConstraint>();
    }

    public void RebuildWing()
    {
        Clear();

        Vector3[] points_global = GetPoints(Vector3.zero, new Vector3(Span, 0, 0), Inner_Segment_Count, Span_Power);
        Vector3[] points_relative = ConvertToRelative(points_global);

        float[] chords = GetValueAtEndpoint(Chord_Base, Chord_Tip, Chord_Power, points_global, Span);
        float[] lifts = GetValueAtEndpoint(Lift_Base, Lift_Tip, Lift_Power, points_global, Span);
        float[] vert_stabilizers = GetValueAtEndpoint(VerticalStabilizer_Base, VerticalStabilizer_Tip, VerticalStabilizer_Power, points_global, Span);

        // These are the anchor points of each wing segment.  They need to stay unscaled (driven home by the name)
        // These act as parents for each wing segment, and are manipulated by the IK animations
        GameObject[] unscaled = CreateUnscaled(points_relative);

        // Just for visualization, shows the chain that the IK animation manipulates
        PopulateBoneRenderTransforms(unscaled);

        CreateWings(unscaled, chords, lifts);
        CreateVerticalStabilizers(unscaled, chords, vert_stabilizers);

        // Wire up IK contraints
        BindIKConstraints(unscaled, points_relative);
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

    private GameObject[] CreateWings(GameObject[] unscaled, float[] chords, float[] lifts)
    {
        var retVal = new GameObject[unscaled.Length - 1];        // there is one more unscaled at the end (located where the last wingtip ends)

        for (int i = 0; i < retVal.Length; i++)
        {
            retVal[i] = Instantiate(Wing_Prefab, unscaled[i].transform);

            // ------------ Position / Size ------------
            Vector3 span = unscaled[i + 1].transform.localPosition;
            Vector3 mid_point = span / 2;

            retVal[i].transform.localPosition = mid_point;

            retVal[i].transform.localScale = new Vector3(
                span.magnitude,
                retVal[i].transform.localScale.y,
                (chords[i] + chords[i + 1]) / 2);

            // ------------ Aero Specific ------------
            var aero = retVal[i].GetComponent<AeroSurface>();

            aero.LiftScale = (lifts[i] + lifts[i + 1]) / 2;

            //aero.ShowDebugLines_Structure = true;
        }

        return retVal;
    }

    private GameObject[] CreateVerticalStabilizers(GameObject[] unscaled, float[] chords, float[] vert_stabilizers)
    {
        var retVal = new List<GameObject>();

        for (int i = 0; i < unscaled.Length; i++)
        {
            if (vert_stabilizers[i] < MIN_VERTICALSTABILIZER_HEIGHT)
                continue;

            GameObject stabilizer = Instantiate(Wing_Prefab, unscaled[i].transform);
            retVal.Add(stabilizer);

            // ------------ Position / Size ------------

            //stabilizer.transform.localPosition = Vector3.zero;     // it sits at the joint, not halfway between joints

            stabilizer.transform.localRotation = Quaternion.Euler(0, 0, 90);

            stabilizer.transform.localScale = new Vector3(
                vert_stabilizers[i],
                stabilizer.transform.localScale.y,
                chords[i]);

            // ------------ Aero Specific ------------

            var aero = stabilizer.GetComponent<AeroSurface>();
            //aero.LiftScale = 0;
            aero.ShowDebugLines_Structure = true;
        }

        return retVal.ToArray();
    }

    private void BindIKConstraints(GameObject[] unscaled, Vector3[] points_relative)
    {
        foreach (var twist in _twist_constraints ?? new TwistChainConstraint[0])
        {
            twist.data.root = unscaled[0].transform;
            twist.data.tip = unscaled[^1].transform;
        }

        _rigBuilder.Build();
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

    private static float[] GetValueAtEndpoint(float val_base, float val_tip, float power, Vector3[] points_global, float total_span)
    {
        var retVal = new float[points_global.Length];

        retVal[0] = val_base;
        retVal[^1] = val_tip;

        for (int i = 1; i < retVal.Length - 1; i++)
        {
            float percent = (points_global[i] - points_global[0]).magnitude / total_span;

            float mult = Mathf.Pow(percent, power);

            retVal[i] = UtilityMath.GetScaledValue(val_base, val_tip, 0, 1, mult);
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

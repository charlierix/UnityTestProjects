using PerfectlyNormalUnity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace Assets.Scripts.Genetic
{
    public static class WingBuilder
    {
        private const string UNSCALED = "unscaled";

        public static (GameObject[] unscaled, GameObject[] wings_horz, GameObject[] wings_vert) BuildWing(WingDefinition def, GameObject mount_point, GameObject wing_prefab, bool is_right)
        {
            // Calculate all the positions
            var endpoints = GetEndpoints(def.Span, is_right);

            Vector3[] points_global = GetPoints(endpoints.from, endpoints.to, def.Inner_Segment_Count, def.Span_Power);
            Vector3[] points_relative = ConvertToRelative(points_global);

            float[] chords = GetValueAtEndpoint(def.Chord_Base, def.Chord_Tip, def.Chord_Power, points_global, def.Span);
            float[] lifts = GetValueAtEndpoint(def.Lift_Base, def.Lift_Tip, def.Lift_Power, points_global, def.Span);
            float[] vert_stabilizers = GetValueAtEndpoint(def.VerticalStabilizer_Base, def.VerticalStabilizer_Tip, def.VerticalStabilizer_Power, points_global, def.Span);

            // Extract some components out of the game objects
            BoneRenderer boneRenderer = mount_point.GetComponent<BoneRenderer>();
            RigBuilder rigBuilder = mount_point.GetComponent<RigBuilder>();
            TwistChainConstraint[] twist_constraints = mount_point.GetComponentsInChildren<TwistChainConstraint>();

            // Remove existing wing from the mount point
            Clear(mount_point, boneRenderer, rigBuilder, twist_constraints);

            // These are the anchor points of each wing segment.  They need to stay unscaled (driven home by the name)
            // These act as parents for each wing segment, and are manipulated by the IK animations
            GameObject[] unscaled = CreateUnscaled(mount_point, points_relative);

            // Make the actual aero surfaces (the objects that get called in fixed update and apply forces to rigid body)
            GameObject[] wings_horz = CreateWings(wing_prefab, unscaled, chords, lifts);
            GameObject[] wings_vert = CreateVerticalStabilizers(wing_prefab, unscaled, chords, vert_stabilizers, def.MIN_VERTICALSTABILIZER_HEIGHT);

            // Just for visualization, shows the chain that the IK animation manipulates
            PopulateBoneRenderTransforms(boneRenderer, unscaled);

            // Wire up IK contraints
            BindIKConstraints(rigBuilder, twist_constraints, unscaled, points_relative);

            return (unscaled, wings_horz, wings_vert);
        }

        // ------------------------------- Add / Remove -------------------------------

        private static void Clear(GameObject mount_point, BoneRenderer boneRenderer, RigBuilder rigBuilder, TwistChainConstraint[] twist_constraints)
        {
            // Clear uscaled (keep Rig)
            foreach (Transform child_transform in mount_point.transform)
            {
                GameObject child = child_transform.gameObject;

                if (child.name.StartsWith(UNSCALED))
                    UnityEngine.Object.Destroy(child);
            }

            // Clear bone renderer's transforms
            boneRenderer.transforms = new Transform[0];

            // Clear IK constraints
            foreach (var twist in twist_constraints ?? new TwistChainConstraint[0])
            {
                twist.data.root = null;
                twist.data.tip = null;
            }

            rigBuilder.Build();
        }

        private static GameObject[] CreateUnscaled(GameObject mount_point, Vector3[] points_relative)
        {
            var retVal = new GameObject[points_relative.Length];

            GameObject parent = mount_point;

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

        private static GameObject[] CreateWings(GameObject wing_prefab, GameObject[] unscaled, float[] chords, float[] lifts)
        {
            var retVal = new GameObject[unscaled.Length - 1];        // there is one more unscaled at the end (located where the last wingtip ends)

            for (int i = 0; i < retVal.Length; i++)
            {
                retVal[i] = UnityEngine.Object.Instantiate(wing_prefab, unscaled[i].transform);

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

        private static GameObject[] CreateVerticalStabilizers(GameObject wing_prefab, GameObject[] unscaled, float[] chords, float[] vert_stabilizers, float min_height)
        {
            var retVal = new List<GameObject>();

            for (int i = 0; i < unscaled.Length; i++)
            {
                if (vert_stabilizers[i] < min_height)
                    continue;

                GameObject stabilizer = UnityEngine.Object.Instantiate(wing_prefab, unscaled[i].transform);
                retVal.Add(stabilizer);

                // ------------ Position / Size ------------

                //stabilizer.transform.localPosition = Vector3.zero;     // it sits at the joint, not halfway between joints

                stabilizer.transform.localRotation = Quaternion.Euler(0, 0, 90);

                stabilizer.transform.localScale = new Vector3(
                    vert_stabilizers[i],
                    stabilizer.transform.localScale.y,
                    chords[i]);

                // ------------ Aero Specific ------------

                //var aero = stabilizer.GetComponent<AeroSurface>();
                //aero.LiftScale = 0;       // already zero
                //aero.ShowDebugLines_Structure = true;
            }

            return retVal.ToArray();
        }

        private static void PopulateBoneRenderTransforms(BoneRenderer boneRenderer, GameObject[] unscaled)
        {
            boneRenderer.transforms = new Transform[unscaled.Length];

            for (int i = 0; i < unscaled.Length; i++)
            {
                boneRenderer.transforms[i] = unscaled[i].transform;
            }
        }

        private static void BindIKConstraints(RigBuilder rigBuilder, TwistChainConstraint[] twist_constraints, GameObject[] unscaled, Vector3[] points_relative)
        {
            foreach (var twist in twist_constraints ?? new TwistChainConstraint[0])
            {
                twist.data.root = unscaled[0].transform;
                twist.data.tip = unscaled[^1].transform;
            }

            rigBuilder.Build();
        }

        // ------------------------------- Calculations -------------------------------

        private static (Vector3 from, Vector3 to) GetEndpoints(float span, bool is_right)
        {
            float to_x = is_right ?
                span :
                -span;

            return (Vector3.zero, new Vector3(to_x, 0, 0));
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
    }
}

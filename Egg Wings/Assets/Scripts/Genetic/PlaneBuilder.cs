﻿using Assets.Scripts.Genetic.Models;
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
    public static class PlaneBuilder
    {
        private const string UNSCALED = "unscaled";

        public static PlaneBuilderResults_Plane BuildPlane(PlaneDefinition def, PlaneBuilder_MountPoints mountpoints, GameObject engine_prefab, GameObject wing_prefab)
        {
            // Throw out items that are too small
            def = RemoveSmallDefinitions.ExaminePlane(def);

            //TODO: validate positions
            //  move engines out of the way
            //  apply modifiers to each wing things are too close together
            //      lift at -90 / 0 / 90
            //      drag at -90 / 0 / 90
            //      from/to

            return new PlaneBuilderResults_Plane()
            {
                Engine_0_Left = BuildEngine(def.Engine_0, mountpoints.Engine_0_Left, engine_prefab, false),
                Engine_0_Right = BuildEngine(def.Engine_0, mountpoints.Engine_0_Right, engine_prefab, true),

                Engine_1_Left = BuildEngine(def.Engine_1, mountpoints.Engine_1_Left, engine_prefab, false),
                Engine_1_Right = BuildEngine(def.Engine_1, mountpoints.Engine_1_Right, engine_prefab, true),

                Engine_2_Left = BuildEngine(def.Engine_2, mountpoints.Engine_2_Left, engine_prefab, false),
                Engine_2_Right = BuildEngine(def.Engine_2, mountpoints.Engine_2_Right, engine_prefab, true),

                Wing_0_Left = BuildWing(def.Wing_0, mountpoints.Wing_0_Left, wing_prefab, false),
                Wing_0_Right = BuildWing(def.Wing_0, mountpoints.Wing_0_Right, wing_prefab, true),

                Wing_1_Left = BuildWing(def.Wing_1, mountpoints.Wing_1_Left, wing_prefab, false),
                Wing_1_Right = BuildWing(def.Wing_1, mountpoints.Wing_1_Right, wing_prefab, true),

                Wing_2_Left = BuildWing(def.Wing_2, mountpoints.Wing_2_Left, wing_prefab, false),
                Wing_2_Right = BuildWing(def.Wing_2, mountpoints.Wing_2_Right, wing_prefab, true),

                //TODO: IF Mathf.Abs(def.Tail.Offset.x) > MIN THEN two tails ELSE one tail with offset.x = 0
                Tail = BuildTail(def.Tail, mountpoints.Tail, wing_prefab),
            };
        }

        public static PlaneBuilderResults_Engine BuildEngine(EngineDefinition def, GameObject mount_point, GameObject engine_prefab, bool is_right)
        {
            if(def == null)
            {
                Clear(mount_point);
                return null;
            }

            SetLeftRightTransform(mount_point, def.Offset, def.Rotation, is_right);

            Clear(mount_point);

            GameObject[] unscaled = CreateUnscaled(mount_point, new[] { Vector3.zero });

            GameObject engine = CreateEngine(engine_prefab, unscaled[0], def.Size);

            return new PlaneBuilderResults_Engine()
            {
                Unscaled = unscaled[0],
                Engine = engine,
            };
        }

        public static PlaneBuilderResults_Wing BuildWing(WingDefinition def, GameObject mount_point, GameObject wing_prefab, bool is_right)
        {
            // Extract some components out of the game objects
            BoneRenderer boneRenderer = mount_point.GetComponent<BoneRenderer>();
            RigBuilder rigBuilder = mount_point.GetComponent<RigBuilder>();
            TwistChainConstraint[] twist_constraints = mount_point.GetComponentsInChildren<TwistChainConstraint>();

            if (def == null)
            {
                Clear(mount_point, boneRenderer, rigBuilder, twist_constraints);
                return null;
            }

            SetLeftRightTransform(mount_point, def.Offset, def.Rotation, is_right);

            // Calculate all the positions
            var endpoints = WingBuilder_Calculations.GetEndpoints_Wing(def.Span, is_right);

            Vector3[] points_global = WingBuilder_Calculations.GetPoints(endpoints.from, endpoints.to, def.Inner_Segment_Count, def.Span_Power);
            Vector3[] points_relative = WingBuilder_Calculations.ConvertToRelative(points_global);

            float[] chords = WingBuilder_Calculations.GetValueAtEndpoint_Power(def.Chord_Base, def.Chord_Tip, def.Chord_Power, points_global, def.Span);
            float[] lifts = WingBuilder_Calculations.GetValueAtEndpoint_Power(def.Lift_Base, def.Lift_Tip, def.Lift_Power, points_global, def.Span);
            float[] vert_stabilizers = WingBuilder_Calculations.GetValueAtEndpoint_Power(def.VerticalStabilizer_Base, def.VerticalStabilizer_Tip, def.VerticalStabilizer_Power, points_global, def.Span);

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

            return new PlaneBuilderResults_Wing()
            {
                Unscaled = unscaled,
                Wings_Horz = wings_horz,
                Wings_Vert = wings_vert,
            };
        }

        public static PlaneBuilderResults_Tail BuildTail(TailDefinition def, GameObject mount_point, GameObject wing_prefab)
        {
            // Extract some components out of the game objects
            BoneRenderer boneRenderer = mount_point.GetComponent<BoneRenderer>();
            RigBuilder rigBuilder = mount_point.GetComponent<RigBuilder>();
            TwistChainConstraint[] twist_constraints = mount_point.GetComponentsInChildren<TwistChainConstraint>();

            if (def == null)
            {
                Clear(mount_point, boneRenderer, rigBuilder, twist_constraints);
                return null;
            }

            mount_point.transform.localPosition = def.Offset;
            mount_point.transform.localRotation = def.Rotation;

            var defB = def.Boom;
            var defT = def.Tail;

            var tail_usage = WingBuilder_Calculations.GetTailUsage(defT.Chord, defT.Horz_Span, defT.Vert_Height, defT.MIN_SIZE);

            // Calculate all the positions
            var endpoints = WingBuilder_Calculations.GetEndpoints_Tail(defB.Length, tail_usage.horz || tail_usage.vert, defT.Chord);

            Vector3[] points_boom_global = WingBuilder_Calculations.GetPoints(endpoints.boom_from, endpoints.boom_to, defB.Inner_Segment_Count, defB.Length_Power);
            Vector3[] points_boom_relative = WingBuilder_Calculations.ConvertToRelative(points_boom_global);

            Vector3[] points_all_global = points_boom_global;
            Vector3[] points_all_relative = points_boom_relative;
            if (tail_usage.horz || tail_usage.vert)
            {
                points_all_global = UtilityCore.ArrayAdd(points_all_global, endpoints.tail_to.Value);
                points_all_relative = WingBuilder_Calculations.ConvertToRelative(points_all_global);
            }

            float[] spans_boom = WingBuilder_Calculations.GetValueAtEndpoint_Bezier(defB.Span_Base, defB.Span_Mid, defB.Span_Tip, defB.Mid_Length, defB.Length, points_boom_global, defB.Bezier_PinchPercent);
            float[] verts_boom = WingBuilder_Calculations.GetValueAtEndpoint_Bezier(defB.Vert_Base, defB.Vert_Mid, defB.Vert_Tip, defB.Mid_Length, defB.Length, points_boom_global, defB.Bezier_PinchPercent);

            // Remove existing wing from the mount point
            Clear(mount_point, boneRenderer, rigBuilder, twist_constraints);

            // These are the anchor points of each wing segment.  They need to stay unscaled (driven home by the name)
            // These act as parents for each wing segment, and are manipulated by the IK animations
            GameObject[] unscaled_all = CreateUnscaled(mount_point, points_all_relative);
            GameObject[] unscaled_boom = unscaled_all;
            if (tail_usage.horz || tail_usage.vert)
            {
                unscaled_boom = unscaled_all.
                    SkipLast(1).
                    ToArray();
            }

            // Make the actual aero surfaces (the objects that get called in fixed update and apply forces to rigid body)
            GameObject[] wings_boom_horz = CreateBoom_Horz(wing_prefab, unscaled_boom, spans_boom);
            GameObject[] wings_boom_vert = CreateBoom_Vert(wing_prefab, unscaled_boom, verts_boom);

            var wings_tail = CreateWings_Tail(wing_prefab, unscaled_all, tail_usage.horz, tail_usage.vert, defT.Horz_Span, defT.Vert_Height);

            // Just for visualization, shows the chain that the IK animation manipulates
            PopulateBoneRenderTransforms(boneRenderer, unscaled_all);

            // Wire up IK contraints
            BindIKConstraints(rigBuilder, twist_constraints, unscaled_all, points_all_relative);

            return new PlaneBuilderResults_Tail()
            {
                Unscaled = unscaled_all,

                Wings_Boom_Horz = wings_boom_horz,
                Wings_Boom_Vert = wings_boom_vert,

                Wing_Tail_Horz = wings_tail.horz,
                Wing_Tail_Vert = wings_tail.vert,
            };
        }

        // ---------------------------------- Common ----------------------------------

        private static void Clear(GameObject mount_point, BoneRenderer boneRenderer = null, RigBuilder rigBuilder = null, TwistChainConstraint[] twist_constraints = null)
        {
            // Clear IK constraints
            if (twist_constraints != null)
            {
                foreach (var twist in twist_constraints)
                {
                    twist.data.root = null;
                    twist.data.tip = null;
                }
            }

            // Clear bone renderer's transforms
            if (boneRenderer != null)
                boneRenderer.transforms = new Transform[0];

            if (rigBuilder != null)
                rigBuilder.Build();

            // Clear uscaled (keep Rig)
            foreach (Transform child_transform in mount_point.transform)
            {
                GameObject child = child_transform.gameObject;

                if (child.name.StartsWith(UNSCALED))
                    UnityEngine.Object.Destroy(child);
            }
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

        private static void SetLeftRightTransform(GameObject mount_point, Vector3 offset, Quaternion rotation, bool is_right)
        {
            mount_point.transform.localPosition = is_right ?
                offset :
                new Vector3(-offset.x, offset.y, offset.z);

            mount_point.transform.localRotation = is_right ?
                rotation :
                Math3D.GetMirroredRotation(rotation, AxisDim.X);
        }

        // ---------------------------------- Engine ----------------------------------

        private static GameObject CreateEngine(GameObject engine_prefab, GameObject unscaled, float size)
        {
            GameObject retVal = UnityEngine.Object.Instantiate(engine_prefab, unscaled.transform);

            retVal.transform.localScale = new Vector3(
                Engine.STANDARD_RADIUS * size,
                Engine.STANDARD_HEIGHT * size,
                Engine.STANDARD_RADIUS * size);

            return retVal;
        }

        // ----------------------------------- Wing -----------------------------------

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

        // ----------------------------------- Tail -----------------------------------

        private static GameObject[] CreateBoom_Horz(GameObject wing_prefab, GameObject[] unscaled, float[] spans)
        {
            var retVal = new GameObject[unscaled.Length - 1];        // there is one more unscaled at the end

            for (int i = 0; i < retVal.Length; i++)
            {
                retVal[i] = UnityEngine.Object.Instantiate(wing_prefab, unscaled[i].transform);

                // ------------ Position / Size ------------

                Vector3 chord = unscaled[i + 1].transform.localPosition;
                Vector3 mid_point = chord / 2;

                retVal[i].transform.localPosition = mid_point;

                retVal[i].transform.localScale = new Vector3(
                    (spans[i] + spans[i + 1]) / 2,
                    retVal[i].transform.localScale.y,
                    chord.magnitude);

                // ------------ Aero Specific ------------

                //var aero = retVal[i].GetComponent<AeroSurface>();
                //aero.LiftScale = 0;       // already zero
                //aero.ShowDebugLines_Structure = true;
            }

            return retVal;
        }

        private static GameObject[] CreateBoom_Vert(GameObject wing_prefab, GameObject[] unscaled, float[] verts)
        {
            var retVal = new GameObject[unscaled.Length - 1];        // there is one more unscaled at the end

            for (int i = 0; i < retVal.Length; i++)
            {
                retVal[i] = UnityEngine.Object.Instantiate(wing_prefab, unscaled[i].transform);

                // ------------ Position / Size ------------

                Vector3 chord = unscaled[i + 1].transform.localPosition;
                Vector3 mid_point = chord / 2;

                retVal[i].transform.localPosition = mid_point;

                retVal[i].transform.localRotation = Quaternion.Euler(0, 0, 90);

                retVal[i].transform.localScale = new Vector3(
                    (verts[i] + verts[i + 1]) / 2,
                    retVal[i].transform.localScale.y,
                    chord.magnitude);

                // ------------ Aero Specific ------------

                //var aero = retVal[i].GetComponent<AeroSurface>();
                //aero.LiftScale = 0;       // already zero
                //aero.ShowDebugLines_Structure = true;
            }

            return retVal;
        }

        private static (GameObject horz, GameObject vert) CreateWings_Tail(GameObject wing_prefab, GameObject[] unscaled, bool has_horz, bool has_vert, float horz_span, float vert_height)
        {
            GameObject horz = null;
            GameObject vert = null;

            if (has_horz)
            {
                horz = UnityEngine.Object.Instantiate(wing_prefab, unscaled[^2].transform);

                // ------------ Position / Size ------------

                Vector3 chord = unscaled[^1].transform.localPosition;
                Vector3 mid_point = chord / 2;

                horz.transform.localPosition = mid_point;

                horz.transform.localScale = new Vector3(
                    horz_span,
                    horz.transform.localScale.y,
                    chord.magnitude);

                // ------------ Aero Specific ------------

                //var aero = horz.GetComponent<AeroSurface>();
                //aero.LiftScale = 0;       // already zero
                //aero.ShowDebugLines_Structure = true;
            }

            if (has_vert)
            {
                vert = UnityEngine.Object.Instantiate(wing_prefab, unscaled[^2].transform);

                // ------------ Position / Size ------------

                Vector3 chord = unscaled[^1].transform.localPosition;
                Vector3 mid_point = chord / 2;

                vert.transform.localPosition = mid_point;

                vert.transform.localRotation = Quaternion.Euler(0, 0, 90);

                vert.transform.localScale = new Vector3(
                    vert_height,
                    vert.transform.localScale.y,
                    chord.magnitude);

                // ------------ Aero Specific ------------

                //var aero = vert.GetComponent<AeroSurface>();
                //aero.LiftScale = 0;       // already zero
                //aero.ShowDebugLines_Structure = true;
            }

            return (horz, vert);
        }
    }
}

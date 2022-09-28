using PerfectlyNormalUnity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Genetic
{
    public static class WingBuilder_Calculations
    {
        public static (Vector3 from, Vector3 to) GetEndpoints_Wing(float span, bool is_right)
        {
            float to_x = is_right ?
                span :
                -span;

            return (Vector3.zero, new Vector3(to_x, 0, 0));
        }

        public static (Vector3 boom_from, Vector3 boom_to, Vector3? tail_to) GetEndpoints_Tail(float boom_length, bool has_tail, float tail_length)
        {
            Vector3 to = new Vector3(0, 0, -boom_length);

            Vector3? tail = has_tail ?
                to + new Vector3(0, 0, -tail_length) :
                null;

            return (Vector3.zero, to, tail);
        }

        public static Vector3[] GetPoints(Vector3 from, Vector3 to, int internal_points, float pow)
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

        public static Vector3[] ConvertToRelative(Vector3[] points_global)
        {
            var retVal = new Vector3[points_global.Length];

            retVal[0] = Vector3.zero;

            for (int i = 0; i < points_global.Length - 1; i++)
            {
                retVal[i + 1] = points_global[i + 1] - points_global[i];
            }

            return retVal;
        }

        public static float[] GetValueAtEndpoint_Power(float val_base, float val_tip, float power, Vector3[] points_global, float total_span)
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
        public static float[] GetValueAtEndpoint_Bezier(float val_base, float val_mid, float val_tip, float mid_dist, float total_span, Vector3[] points_global, float pinch_percent)
        {
            var retVal = new float[points_global.Length];

            retVal[0] = val_base;
            retVal[^1] = val_tip;

            // Only the Y is reported (X is used to help define the curve)
            var bezier = BezierUtil.GetBezierSegments(new[] { new Vector3(0, val_base, 0), new Vector3(mid_dist, val_mid, 0), new Vector3(total_span, val_tip, 0) }, pinch_percent);

            var get_x_at_dist = new Func<float, float>(perc => BezierUtil.GetPoint(perc, bezier).x);

            for (int i = 1; i < retVal.Length - 1; i++)
            {
                float dist_desired = (points_global[i] - points_global[0]).magnitude;

                float dist_actual = Math1D.GetInputForDesiredOutput_PosInput_PosCorrelation(dist_desired, 0.01f, get_x_at_dist);

                Vector3 bez_point = BezierUtil.GetPoint(dist_actual, bezier);

                retVal[i] = bez_point.y;
            }

            return retVal;
        }

        public static (bool horz, bool vert) GetTailUsage(float chord, float horz_span, float vert_height, float min_size)
        {
            if (chord <= min_size)
                return (false, false);

            bool horz = horz_span >= min_size;
            bool vert = vert_height >= min_size;

            return (horz, vert);
        }
    }
}

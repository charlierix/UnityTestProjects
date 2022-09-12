using PerfectlyNormalUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// https://github.com/brihernandez/SimpleWings/blob/master/Assets/SimpleWings/Scripts/Editor/WingCurvesEditor.cs

public static class AeroCurveBuilder
{
    #region class: LiftProps

    private class LiftProps
    {
        // -------------------- LIFT --------------------

        // lift at zero AOA
        public float neutralLift = 0.0f;        // lift generated when the wing as it zero angle of attack

        // lift at critical AOA
        public float maxLiftPositive = 1.1f;        // lift coeffient at a positive, critical angle of attack, when the wing is generating the most lift

        // lift when fully stalled
        public float minLiftPositive = 0.6f;        // lift coeffient when the wing is fully stalled at a positive angle of attack

        // -------------------- ANGLE --------------------

        // negative AOA multiplier
        public float negativeAoaMult = 1.0f;        // multiplier for lift generated before stall when at negative angles of attack

        // flat plate lift multiplier
        public float flatPlateMult = 1.0f;      // multiplier for the flat plate lift that occurs for any wing from 45 to 135 degrees of rotation

        // critical AOA (1 - 35)
        public float criticalAngle = 16.0f;     // critical angle of attack is both the angle at which the wing starts to stall, and the angle at which it produces the most lift

        // fully stalled AOA (2 - 44)
        public float fullyStalledAngle = 20.0f;     // angle of attack at which the wing is fully stalled and producing the minimum lift

        // -------------------- OTHER --------------------

        public float flatPlateMax = 0.9f;
    }

    #endregion
    #region class: DragProps

    private class DragProps
    {
        public float drag_0 = 0;
        public float drag_90 = 1;
        public float drag_180 = 0;
    }

    #endregion

    private const float MID = 0.4f;

    // Percent of 0 is a flat plate, 1 is a high lift/drag wing
    public static AnimationCurve GetLiftByPercent(float percent)
    {
        LiftProps props = GetLiftProps_LERP(percent);

        return GetLiftCurveFromProps(props);
    }
    public static AnimationCurve GetDragByPercent(float percent)
    {
        DragProps props = GetDragProps_LERP(percent);

        return GetDragCurveFromProps(props);
    }

    private static AnimationCurve GetLiftCurveFromProps(LiftProps lp)
    {
        // Error checking. Prevent keys from going in out of order and try to maintain a sorta normal line.
        if (lp.fullyStalledAngle < lp.criticalAngle)
            lp.fullyStalledAngle = lp.criticalAngle + 0.1f;

        if (lp.neutralLift > lp.maxLiftPositive)
            lp.neutralLift = lp.maxLiftPositive - 0.01f;

        lp.flatPlateMult = Mathf.Clamp(lp.flatPlateMult, 0.0f, 100.0f);

        return new AnimationCurve()
        {
            keys = new[]
            {
			    // Wing at positive AOA
			    new Keyframe(0.0f, lp.neutralLift),
                new Keyframe(lp.criticalAngle, lp.maxLiftPositive),     // 0 to here is the normal lift region
			    new Keyframe(lp.fullyStalledAngle, lp.minLiftPositive),     // critical to here is where it's slipping

			    // Flat plate, generic across all wings
			    new Keyframe(45.0f, lp.flatPlateMax * lp.flatPlateMult),
                new Keyframe(90.0f, 0.0f),
                new Keyframe(135.0f, -lp.flatPlateMax * lp.flatPlateMult),

			    // Wing at negative AOA
			    new Keyframe(180.0f - lp.fullyStalledAngle, -lp.minLiftPositive * lp.negativeAoaMult),
                new Keyframe(180.0f - lp.criticalAngle, -lp.maxLiftPositive * lp.negativeAoaMult),
                new Keyframe(180.0f, -lp.neutralLift * lp.negativeAoaMult)
            },
        };
    }
    private static AnimationCurve GetDragCurveFromProps(DragProps dp)
    {
        return new AnimationCurve()
        {
            keys = new[]
            {
                new Keyframe(0, dp.drag_0),
                new Keyframe(90, dp.drag_90),
                new Keyframe(180, dp.drag_180),
            },
        };
    }

    private static LiftProps GetLiftProps_LERP(float percent)
    {
        if (percent < 0 || percent.IsNearZero())
            return GetLiftProps_Flat();

        if (percent > 1 || percent.IsNearValue(1))
            return GetLiftProps_HighLift();

        if (percent.IsNearValue(MID))
            return GetLiftProps_Standard();

        LiftProps low, high;
        float scaled_percent;

        if (percent < MID)
        {
            low = GetLiftProps_Flat();
            high = GetLiftProps_Standard();
            scaled_percent = UtilityMath.GetScaledValue(0, 1, 0, MID, percent);
        }
        else
        {
            low = GetLiftProps_Standard();
            high = GetLiftProps_HighLift();
            scaled_percent = UtilityMath.GetScaledValue(0, 1, MID, 1, percent);
        }

        return new LiftProps()
        {
            neutralLift = UtilityMath.GetScaledValue(low.neutralLift, high.neutralLift, 0, 1, scaled_percent),

            criticalAngle = UtilityMath.GetScaledValue(low.criticalAngle, high.criticalAngle, 0, 1, scaled_percent),
            maxLiftPositive = UtilityMath.GetScaledValue(low.maxLiftPositive, high.maxLiftPositive, 0, 1, scaled_percent),

            fullyStalledAngle = UtilityMath.GetScaledValue(low.fullyStalledAngle, high.fullyStalledAngle, 0, 1, scaled_percent),
            minLiftPositive = UtilityMath.GetScaledValue(low.minLiftPositive, high.minLiftPositive, 0, 1, scaled_percent),

            flatPlateMult = UtilityMath.GetScaledValue(low.flatPlateMult, high.flatPlateMult, 0, 1, scaled_percent),
            negativeAoaMult = UtilityMath.GetScaledValue(low.negativeAoaMult, high.negativeAoaMult, 0, 1, scaled_percent),

            flatPlateMax = UtilityMath.GetScaledValue(low.flatPlateMax, high.flatPlateMax, 0, 1, scaled_percent),
        };
    }
    private static DragProps GetDragProps_LERP(float percent)
    {
        if (percent < 0 || percent.IsNearZero())
            return GetDragProps_Flat();

        if (percent > 1 || percent.IsNearValue(1))
            return GetDragProps_HighLift();

        if (percent.IsNearValue(MID))
            return GetDragProps_Standard();

        DragProps low, high;
        float scaled_percent;

        if (percent < MID)
        {
            low = GetDragProps_Flat();
            high = GetDragProps_Standard();
            scaled_percent = UtilityMath.GetScaledValue(0, 1, 0, MID, percent);
        }
        else
        {
            low = GetDragProps_Standard();
            high = GetDragProps_HighLift();
            scaled_percent = UtilityMath.GetScaledValue(0, 1, MID, 1, percent);
        }

        return new DragProps()
        {
            drag_0 = UtilityMath.GetScaledValue(low.drag_0, high.drag_0, 0, 1, scaled_percent),
            drag_90 = UtilityMath.GetScaledValue(low.drag_90, high.drag_90, 0, 1, scaled_percent),
            drag_180 = UtilityMath.GetScaledValue(low.drag_180, high.drag_180, 0, 1, scaled_percent),
        };
    }

    private static LiftProps GetLiftProps_Flat()
    {
        return new LiftProps()
        {
            neutralLift = 0,

            criticalAngle = 15,
            maxLiftPositive = 1f / 3f,

            fullyStalledAngle = 30,
            minLiftPositive = 2f / 3f,

            flatPlateMult = 1f,
            negativeAoaMult = 1f,

            flatPlateMax = 1f
        };
    }
    private static LiftProps GetLiftProps_Standard()
    {
        return new LiftProps()
        {
            neutralLift = 0,

            criticalAngle = 16,
            maxLiftPositive = 1.1f,

            fullyStalledAngle = 20,
            minLiftPositive = 0.6f,

            flatPlateMult = 1f,
            negativeAoaMult = 1f,

            flatPlateMax = 0.9f,
        };
    }
    private static LiftProps GetLiftProps_HighLift()
    {
        return new LiftProps()
        {
            neutralLift = 0.25f,

            criticalAngle = 12,
            maxLiftPositive = 1.4f,

            fullyStalledAngle = 16,
            minLiftPositive = 0.6f,

            flatPlateMult = 1f,
            negativeAoaMult = 0.5f,

            flatPlateMax = 0.9f,
        };
    }

    private static DragProps GetDragProps_Flat()
    {
        return new DragProps()
        {
            drag_0 = 0.025f,
            drag_90 = 2,
            drag_180 = 0.025f,
        };
    }
    private static DragProps GetDragProps_Standard()
    {
        return new DragProps()
        {
            drag_0 = 0.025f,
            drag_90 = 1.8f,
            drag_180 = 0.025f,
        };
    }
    private static DragProps GetDragProps_HighLift()
    {
        return new DragProps()
        {
            drag_0 = 0.025f,
            drag_90 = 1,
            drag_180 = 0.025f,
        };
    }
}

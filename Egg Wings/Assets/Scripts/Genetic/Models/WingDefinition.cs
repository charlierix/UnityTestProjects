using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Genetic.Models
{
    public record WingDefinition
    {
        public Vector3 Offset { get; init; }        // this is for the right wing.  The left will be mirroed
        public Quaternion Rotation { get; init; }

        /// <summary>
        /// There will be a root point, this many interior points, tip point
        /// </summary>
        /// <remarks>
        /// This doesn't change the total length of the wing, just how much it is chopped up
        /// 
        /// Each * is an unscaled anchor point
        /// Each - is a wing segment (vertical stabilizers are tied to the anchor points)
        /// 
        /// 0:
        ///     *-*
        /// 
        /// 1:
        ///     *-*-*
        /// 
        /// 2:
        ///     *-*-*-*
        ///     
        /// 3:
        ///     *-*-*-*-*
        /// </remarks>
        public int Inner_Segment_Count { get; init; } = 2;

        /// <summary>
        /// Total length of the wing (wing span)
        /// X scale
        /// </summary>
        public float Span { get; init; } = 1f;
        /// <summary>
        /// How the wing segments are spaced apart
        /// </summary>
        /// <remarks>
        /// If power is 1, then they are spaced linearly:
        /// *     *     *     *
        /// 
        /// If power is .5, then it's sqrt
        /// *       *     *   *
        /// 
        /// If power is 2, then it's ^2 (this would be an unnatural way to make a wing)
        /// *   *     *       *
        /// </remarks>
        public float Span_Power { get; init; } = 1f;       // all inputs to the power function are scaled from 0 to 1.  You generally want to set power between 0.5 and 1

        /// <summary>
        /// The part the runs parallel to the fuselage
        /// Z scale
        /// </summary>
        public float Chord_Base { get; init; } = 0.4f;
        public float Chord_Tip { get; init; } = 0.4f;
        public float Chord_Power { get; init; } = 1f;

        /// <summary>
        /// How much lift the wing generates
        /// </summary>
        /// <remarks>
        /// 0 is no extra lift, 1 is high lift/high drag
        /// 
        /// The way lift works is between about -20 to 20 degrees angle of attack, there is extra force applied along the wing's normal
        /// But that comes at a cost of extra drag and less effective at higher angles of attack
        /// </remarks>
        public float Lift_Base { get; init; } = 0.7f;
        public float Lift_Tip { get; init; } = 0.2f;
        public float Lift_Power { get; init; } = 1f;

        public float MIN_VERTICALSTABILIZER_HEIGHT { get; init; } = 0.1f;

        /// <summary>
        /// These are vertical pieces (lift is set to zero)
        /// </summary>
        /// <remarks>
        /// You would generally want a single stabalizer at the tip of the wing.  To accomplish that, use a high power
        /// (the height at each segment is near zero, then height approaches max close to the tip)
        /// </remarks>
        public float VerticalStabilizer_Base { get; init; } = 0;       // NOTE: a vertical stabilizer won't be created for a segment if it's less than MIN_VERTICALSTABILIZER_HEIGHT
        public float VerticalStabilizer_Tip { get; init; } = 0;
        public float VerticalStabilizer_Power { get; init; } = 16;
    }
}

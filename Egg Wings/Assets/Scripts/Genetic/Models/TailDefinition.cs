using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Genetic.Models
{
    public record TailDefinition
    {
        public Vector3 Offset { get; init; }
        public Quaternion Rotation { get; init; }

        public TailDefinition_Boom Boom { get; init; }
        public TailDefinition_Tail Tail { get; init; }
    }

    public record TailDefinition_Boom
    {
        public int Inner_Segment_Count { get; init; } = 2;

        public float Length { get; init; } = 1f;
        public float Length_Power { get; init; } = 1f;

        public float Mid_Length { get; init; } = 0.7f;

        public float Span_Base { get; init; } = 0.5f;
        public float Span_Mid { get; init; } = 0.2f;
        public float Span_Tip { get; init; } = 0.3f;

        public float Vert_Base { get; init; } = 0.5f;
        public float Vert_Mid { get; init; } = 0.2f;
        public float Vert_Tip { get; init; } = 0.3f;

        public float Bezier_PinchPercent { get; init; } = 0.2f;       // 0 to 0.4 (affects the curviness of the bezier, 0 is linear)
    }

    public record TailDefinition_Tail
    {
        public float MIN_SIZE { get; init; } = 0.1f;

        public float Chord { get; init; } = 0.33f;        // if < MIN_TAIL_SIZE, there is no tail section
        public float Horz_Span { get; init; } = 0.5f;     // if < MIN_TAIL_SIZE, there is no horizontal wing
        public float Vert_Height { get; init; } = 0.5f;       // if < MIN_TAIL_SIZE, there is no vertical wing
    }
}

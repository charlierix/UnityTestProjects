using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Genetic
{
    public class TailDefinition
    {
        public TailDefinition_Boom Boom = null;
        public TailDefinition_Tail Tail = null;
    }
    public class TailDefinition_Boom
    {
        public int Inner_Segment_Count = 2;

        public float Length = 1f;
        public float Length_Power = 1f;

        public float Mid_Length = 0.7f;

        public float Span_Base = 0.5f;
        public float Span_Mid = 0.2f;
        public float Span_Tip = 0.3f;

        public float Vert_Base = 0.5f;
        public float Vert_Mid = 0.2f;
        public float Vert_Tip = 0.3f;

        public float Bezier_PinchPercent = 0.2f;       // 0 to 0.4 (affects the curviness of the bezier, 0 is linear)
    }
    public class TailDefinition_Tail
    {
        public float MIN_SIZE = 0.1f;

        public float Chord = 0.33f;        // if < MIN_TAIL_SIZE, there is no tail section
        public float Horz_Span = 0.5f;     // if < MIN_TAIL_SIZE, there is no horizontal wing
        public float Vert_Height = 0.5f;       // if < MIN_TAIL_SIZE, there is no vertical wing
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Genetic.Models
{
    public record WingModifier
    {
        public MultAtAngle Lift { get; init; }
        public MultAtAngle Drag { get; init; }

        public float From { get; init; }
        public float To { get; init; }
    }
}

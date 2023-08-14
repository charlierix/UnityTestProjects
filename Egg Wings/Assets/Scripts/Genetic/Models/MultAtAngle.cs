using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Genetic.Models
{
    public record MultAtAngle
    {
        public float Neg_90 { get; init; } = 1f;
        public float Zero { get; init; } = 1f;
        public float Pos_90 { get; init; } = 1f;
    }
}

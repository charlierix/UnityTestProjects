using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Genetic.Models
{
    public record PlaneDefinition
    {
        public EngineDefinition Engine_0 { get; init; }
        public EngineDefinition Engine_1 { get; init; }
        public EngineDefinition Engine_2 { get; init; }

        public WingDefinition Wing_0 { get; init; }
        public WingDefinition Wing_1 { get; init; }
        public WingDefinition Wing_2 { get; init; }

        public WingModifier[] WingModifiers_0 { get; init; }
        public WingModifier[] WingModifiers_1 { get; init; }
        public WingModifier[] WingModifiers_2 { get; init; }

        public TailDefinition Tail { get; init; }

        // head canard

        // spine
        //  this would be a single hinge joint, or a small chain of segments

        //TODO: maybe some way to define mass
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Genetic.Models
{
    public class PlaneDefinition
    {
        public EngineDefinition Engine_0 { get; set; }
        public EngineDefinition Engine_1 { get; set; }
        public EngineDefinition Engine_2 { get; set; }

        public WingDefinition Wing_0 { get; set; }
        public WingDefinition Wing_1 { get; set; }
        public WingDefinition Wing_2 { get; set; }

        public TailDefinition Tail { get; set; }

        // head canard

        // spine
        //  this would be a single hinge joint, or a small chain of segments

        // extra wings (hand/foot)
        //  if these are used, then the main wing will need to mount higher up to keep non interfering airflow

        //TODO: maybe some way to define mass
    }
}

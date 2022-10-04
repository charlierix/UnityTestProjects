using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Genetic.Models
{
    public class EngineDefinition
    {
        public Vector3 Offset { get; set; }        // this is for the right wing.  The left will be mirroed
        public Quaternion Rotation { get; set; }

        public float Size = 1;
    }
}

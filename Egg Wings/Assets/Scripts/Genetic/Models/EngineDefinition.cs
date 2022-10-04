using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Genetic.Models
{
    public record EngineDefinition
    {
        public Vector3 Offset { get; init; }        // this is for the right wing.  The left will be mirroed
        public Quaternion Rotation { get; init; }

        public float Size { get; init; } = 1;
    }
}

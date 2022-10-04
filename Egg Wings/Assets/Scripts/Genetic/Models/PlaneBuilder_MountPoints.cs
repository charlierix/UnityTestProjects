using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Genetic.Models
{
    public record PlaneBuilder_MountPoints
    {
        public GameObject Engine_0_Left { get; init; }
        public GameObject Engine_0_Right { get; init; }

        public GameObject Engine_1_Left { get; init; }
        public GameObject Engine_1_Right { get; init; }

        public GameObject Engine_2_Left { get; init; }
        public GameObject Engine_2_Right { get; init; }

        public GameObject Wing_0_Left { get; init; }
        public GameObject Wing_0_Right { get; init; }

        public GameObject Wing_1_Left { get; init; }
        public GameObject Wing_1_Right { get; init; }

        public GameObject Wing_2_Left { get; init; }
        public GameObject Wing_2_Right { get; init; }

        public GameObject Tail { get; init; }
    }
}

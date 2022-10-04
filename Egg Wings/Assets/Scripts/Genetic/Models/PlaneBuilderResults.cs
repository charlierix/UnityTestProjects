﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Genetic.Models
{
    public record PlaneBuilderResults_Plane
    {
        public PlaneBuilderResults_Engine Engine_Left { get; init; }
        public PlaneBuilderResults_Engine Engine_Right { get; init; }

        public PlaneBuilderResults_Wing Wing_Left { get; init; }
        public PlaneBuilderResults_Wing Wing_Right { get; init; }

        public PlaneBuilderResults_Tail Tail { get; init; }
    }

    public record PlaneBuilderResults_Engine
    {
        public GameObject Unscaled { get; init; }

        public GameObject Engine { get; init; }
    }

    public record PlaneBuilderResults_Wing
    {
        public GameObject[] Unscaled { get; init; }

        public GameObject[] Wings_Horz { get; init; }
        public GameObject[] Wings_Vert { get; init; }
    }

    public record PlaneBuilderResults_Tail
    {
        public GameObject[] Unscaled { get; init; }

        public GameObject[] Wings_Boom_Horz { get; init; }
        public GameObject[] Wings_Boom_Vert { get; init; }

        // Either of these two could be null
        public GameObject Wing_Tail_Horz { get; init; }
        public GameObject Wing_Tail_Vert { get; init; }
    }
}

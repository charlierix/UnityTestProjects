using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerMoveController_Percents
{
    public float Forward = 1f;
    public float Backward = 1f;
    public float Side = 1f;

    public float GetFrontBack(float frontBack)
    {
        return frontBack >= 0 ?
            Forward :
            Backward;
    }
}

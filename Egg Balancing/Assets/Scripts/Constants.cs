using PerfectlyNormalUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Constants
{
    //public const string COLOR_HEAD = "92F32C";
    //public const string COLOR_LEFT = "23C4DB";
    //public const string COLOR_RIGHT = "E85727";

    //public const float DOT_RADIUS = .006f;
    //public const float LINE_THICKNESS = .002f;

    //public static Color ColorHead => UtilityUnity.ColorFromHex(COLOR_HEAD);
    //public static Color ColorLeft => UtilityUnity.ColorFromHex(COLOR_LEFT);
    //public static Color ColorRight => UtilityUnity.ColorFromHex(COLOR_RIGHT);

    public static int LAYER_GROUND = 1 << LayerMask.NameToLayer("Ground");      // https://forum.unity.com/threads/physics-overlapsphere-layer.51927/
    //public static int LAYER_UI = 1 << LayerMask.NameToLayer("UI");
}

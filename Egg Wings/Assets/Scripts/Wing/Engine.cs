using PerfectlyNormalUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engine : MonoBehaviour
{
    public const float STANDARD_HEIGHT = 0.4f;      // these are what the scale of the engine should be when size is "one"
    public const float STANDARD_DIAMETER = 0.3f;

    public float THRUST_AT_HALF = 36;
    public float THRUST_AT_DOUBLE = 144;

    public float Fire_Percent = 0f;     // 0 to 1

    private Rigidbody _body = null;

    private void Awake()
    {
        _body = GetComponentInParent<Rigidbody>();      // this is recursive
    }

    private void FixedUpdate()
    {
        if (Fire_Percent.IsNearZero())
            return;

        float mult = GetMult();

        Vector3 force = transform.up * mult * Fire_Percent;

        _body.AddForceAtPosition(force, transform.position, ForceMode.Force);
    }

    private float GetMult()
    {
        //float radius = transform.lossyScale.x;
        float height = transform.lossyScale.y;

        //TODO: figure out how mult should grow with scale:
        //  N: based on height of cylinder
        //  N^3: based on volume of cylinder
        //  N^2: somewhere in between
        float scale = height / STANDARD_HEIGHT;

        return UtilityMath.GetScaledValue(THRUST_AT_HALF, THRUST_AT_DOUBLE, 0.5f, 2f, scale);       //NOTE: This version of the scale method isn't clamped
    }
}

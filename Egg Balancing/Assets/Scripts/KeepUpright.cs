using PerfectlyNormalUnity;
using PerfectlyNormalUnity.FollowDirectionPosition;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepUpright : MonoBehaviour
{
    public float LANDINGHEIGHT_MIN = 1f;     //NOTE: calculations are based on neck's position
    public float LANDINGHEIGHT_MAX = 5f;

    public float DOT_MIN = -.92f;      // the dot where min percent is applied
    public float DOT_MAX = .71f;

    public float PERCENT_LANDING = .95f;     // percent to use when landing
    public float PERCENT_FLIGHT_MAX = .3f;
    public float PERCENT_FLIGHT_MIN = .07f;

    public GameObject RigidBodyObject;
    private Rigidbody _body = null;

    public AltitudeRelativeToGround Altitude;

    private FollowOrientation _follow;

    public float Percent { get; private set; }

    private void Start()
    {
        _body = RigidBodyObject.GetComponent<Rigidbody>();

        _follow = new FollowOrientation(_body, FollowOrientation.GetStandard(), Vector3.up);
        _follow.SetOrientation(Vector3.up);
        _follow.Percent = .125f;
    }

    private void FixedUpdate()
    {
        Percent = GetPercent();

        _follow.Percent = Percent;

        _follow.Tick();
    }

    private float GetPercent()
    {
        // See if something is under us
        float? altitude = Altitude.Altitude;
        if (altitude != null && altitude.Value <= LANDINGHEIGHT_MAX)
        {
            if (altitude.Value < LANDINGHEIGHT_MIN)
                return PERCENT_LANDING;

            return UtilityMath.GetScaledValue(PERCENT_FLIGHT_MAX, PERCENT_LANDING, LANDINGHEIGHT_MAX, LANDINGHEIGHT_MIN, altitude.Value);
        }

        float dot = Vector3.Dot(_body.rotation * Vector3.up, Vector3.up);

        // Tipped over too far
        if (dot < DOT_MIN)
            return PERCENT_FLIGHT_MAX;

        // Mostly upright
        if (dot > DOT_MAX)
            return PERCENT_FLIGHT_MIN;

        // In between 
        return UtilityMath.GetScaledValue(PERCENT_FLIGHT_MIN, PERCENT_FLIGHT_MAX, DOT_MAX, DOT_MIN, dot);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AltitudeRelativeToGround : MonoBehaviour
{
    public Transform Body;

    /// <summary>
    /// If a ground object is smaller than the dot product with vector3.down, it will be ignored
    /// </summary>
    public float MinDot = .6f;

    /// <summary>
    /// This is the radius of the search
    /// </summary>
    public float MaxTrackedAltitude = 36f;

    public float? Altitude = null;
    public float Altitude_Debug = -999;

    private void FixedUpdate()
    {
        Altitude = GetFloorDistance();
        Altitude_Debug = Altitude ?? -999;
    }

    private float? GetFloorDistance()
    {
        Vector3 position = Body.position;

        float? retVal = null;

        foreach (var hit in Physics.OverlapSphere(position, MaxTrackedAltitude, Constants.LAYER_GROUND))
        {
            Vector3 closest_point;

            if (hit is TerrainCollider)     //TODO: be more generic and do this for all non convex colliders
            {
                if (!hit.Raycast(new Ray(position, Vector3.down), out RaycastHit ray_hit, MaxTrackedAltitude))
                    continue;

                closest_point = ray_hit.point;
            }
            else
            {
                closest_point = hit.ClosestPoint(position);
            }

            Vector3 direction = closest_point - position;

            if (direction.sqrMagnitude < 1f)
            {
                retVal = direction.sqrMagnitude;
                continue;
            }

            // Don't include if it's not below the player
            if (Vector3.Dot(Vector3.down, direction.normalized) < MinDot)
                continue;

            retVal = direction.sqrMagnitude;
        }

        if (retVal != null)
            return Mathf.Sqrt(retVal.Value);
        else
            return null;
    }
}

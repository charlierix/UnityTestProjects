using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlightStamina
{
    public float Max = 144;
    public float Current = 144;

    public float Cost_InitialJump = 36f;
    public float Cost_Flap = 18f;

    /// <summary>
    /// This is per second
    /// </summary>
    public float Cost_Glide = 36f;

    /// <summary>
    /// This is per second
    /// </summary>
    /// <remarks>
    /// Regeneration only happens when they aren't gliding
    /// </remarks>
    public float Regen_Ground = 108f;
    public float Regen_Falling = 12f;

    public void Refill()
    {
        Current = Max;
    }

    /// <remarks>
    /// Even if no stamina remains, the act of gliding doesn't allow stamina to regen (the player
    /// needs to decide to stop gliding for refills to happen)
    /// </remarks>
    /// <returns>
    /// This only returns false if isGliding is true and there isn't enough stamina
    /// </returns>
    public bool Update(float elapsedSeconds, FlightStamina_Activity activity)
    {
        switch (activity)
        {
            case FlightStamina_Activity.OnGround:
                Current = Math.Min(Max, Current + (Regen_Ground * elapsedSeconds));
                return true;

            case FlightStamina_Activity.Falling:
                Current = Math.Min(Max, Current + (Regen_Falling * elapsedSeconds));
                return true;

            case FlightStamina_Activity.Gliding:
                Current = Math.Max(0f, Current - (Cost_Glide * elapsedSeconds));
                return !Mathf.Approximately(Current, 0f);       // as long as it's not zero, then glide is successful

            default:
                throw new ApplicationException($"Unknown {nameof(FlightStamina_Activity)}: {activity}");
        }
    }

    public bool Jump()
    {
        return TryStoreNewValue(Current - Cost_InitialJump);
    }
    public bool Flap()
    {
        return TryStoreNewValue(Current - Cost_Flap);
    }

    private bool TryStoreNewValue(float newValue)
    {
        if (newValue < 0f)
        {
            return false;
        }
        else
        {
            Current = newValue;
            return true;
        }
    }
}

public enum FlightStamina_Activity
{
    OnGround,
    Falling,
    Gliding,
}

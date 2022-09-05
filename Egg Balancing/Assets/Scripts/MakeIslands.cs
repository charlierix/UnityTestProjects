using PerfectlyNormalUnity;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MakeIslands : MonoBehaviour
{
    public GameObject Template;

    void Start()
    {
        var positions = new List<Vector3>()
        {
            Vector3.zero,
        };

        int count = StaticRandom.Next(24, 72);

        for (int cntr = 0; cntr < count; cntr++)
        {
            Vector3? pos = GetRandomPosition(positions);
            if (pos == null)
                break;

            positions.Add(pos.Value);

            Instantiate(Template, pos.Value, Quaternion.identity);
        }
    }

    private static Vector3? GetRandomPosition(List<Vector3> existing)
    {
        for (int cntr = 0; cntr < 144; cntr++)
        {
            Vector3 retVal = new Vector3(
                StaticRandom.NextFloat(-576, 576),
                StaticRandom.NextFloat(72, 432),
                StaticRandom.NextFloat(-576, 576));

            if (!existing.Any(o => (o - retVal).sqrMagnitude < 36 * 36))
                return retVal;
        }

        return null;
    }
}

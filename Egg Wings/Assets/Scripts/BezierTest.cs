using PerfectlyNormalUnity;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BezierTest : MonoBehaviour
{
    private DebugRenderer3D _renderer = null;

    private List<DebugItem> _items = new List<DebugItem>();

    void Start()
    {
        _renderer = gameObject.AddComponent<DebugRenderer3D>();
    }

    public void TestBezier()
    {
        _renderer.Remove(_items);
        _items.Clear();

        Vector3[] controls = Enumerable.Range(0, StaticRandom.Next(3, 8)).
            Select(o => Math3D.GetRandomVector_Spherical(12)).
            ToArray();

        var bezier = BezierUtil.GetBezierSegments(controls);
        Vector3[] samples = BezierUtil.GetPoints(144, bezier);

        var sizes = DebugRenderer3D.GetDrawSizes(12);

        _items.Add(_renderer.AddDots(controls, sizes.dot, Color.black));

        for (int i = 0; i < samples.Length - 1; i++)
        {
            _items.Add(_renderer.AddLine_Basic(samples[i], samples[i + 1], sizes.line, Color.white));
        }
    }
}

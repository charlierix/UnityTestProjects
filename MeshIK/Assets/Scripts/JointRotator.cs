using PerfectlyNormalUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JointRotator
{
    #region enum: SyncAlgorithm

    private enum SyncAlgorithm
    {
        None,
        Two,
    }

    #endregion

    #region Declaration Section

    /// <summary>
    /// This is the joint to keep rotated based on the endpoint positions
    /// </summary>
    public Transform Joint;

    /// <summary>
    /// These are locations of other joints that are linked to this
    /// </summary>
    public Transform[] Endpoints;

    private SyncAlgorithm _algorithm;

    // ------------------ These are used if it's two

    private Quaternion _delta;

    #endregion

    public void Initialize()
    {
        if (Joint == null || Endpoints == null || Endpoints.Length != 2)
        {
            _algorithm = SyncAlgorithm.None;
            return;
        }

        _algorithm = SyncAlgorithm.Two;

        // Get the bisecting line
        // Get the orth of bisecting lines using one of the two source lines in the cross
        var bisect = GetBisect(Endpoints[0].position, Endpoints[1].position);

        Quaternion bisectRot = Quaternion.LookRotation(bisect.Standard, bisect.Orth);

        _delta = bisectRot * Quaternion.Inverse(Joint.rotation);
    }

    public void SyncToEndpoints()
    {
        if (_algorithm != SyncAlgorithm.Two)
            return;

        var bisect = GetBisect(Endpoints[0].position, Endpoints[1].position);

        Joint.rotation = Quaternion.LookRotation(bisect.Standard, bisect.Orth) * _delta;
    }

    #region Private Methods

    private DoubleVector GetBisect(Vector3 v1, Vector3 v2)
    {
        Vector3 bisect = Math3D.GetAverage(v1, v2).normalized;

        return new DoubleVector
        (
            bisect,
            Vector3.Cross(bisect, v1).normalized
        );
    }

    #endregion
}

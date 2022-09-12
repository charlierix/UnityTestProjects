using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This can be used to visualize the center of mass
/// </summary>
/// <remarks>
/// TODO: Make another class that scales/rotates according to inertia vector
/// </remarks>
public class MoveToCenterMass : MonoBehaviour
{
    public GameObject RigidBodyObject;
    private Rigidbody _body = null;

    void Start()
    {
        _body = RigidBodyObject.GetComponent<Rigidbody>();
    }

    void Update()
    {
        transform.localPosition = _body.centerOfMass;
    }
}

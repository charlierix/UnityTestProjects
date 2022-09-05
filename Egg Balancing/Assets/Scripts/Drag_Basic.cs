using PerfectlyNormalUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drag_Basic : MonoBehaviour
{
    public float SPEED_ZERO = 12;
    public float SPEED_FULL = 36;

    public float DRAG = 0.1f;

    public GameObject RigidBodyObject;
    private Rigidbody _body = null;

    void Start()
    {
        _body = RigidBodyObject.GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Vector3 velocity = _body.velocity;
        float speed_sqr = velocity.sqrMagnitude;

        if (speed_sqr < SPEED_ZERO * SPEED_ZERO)
            return;

        float drag = GetDragPercent(speed_sqr);

        _body.AddForce(velocity * -drag, ForceMode.Acceleration);
    }

    private float GetDragPercent(float speed_sqr)
    {
        if (speed_sqr > SPEED_FULL * SPEED_FULL)
            return DRAG;

        else
            return UtilityMath.GetScaledValue(0, DRAG, SPEED_ZERO, SPEED_FULL, Mathf.Sqrt(speed_sqr));
    }
}

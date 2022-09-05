using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetPlayer : MonoBehaviour
{
    public GameObject Body;
    private Rigidbody _body = null;

    public FlightStartStopTracker StartStopTracker;

    public ControllerListener ControllerListener;

    private void Start()
    {
        _body = Body.GetComponent<Rigidbody>();

        ControllerListener.Down_Left_B.AddListener(Reset);
    }

    private void Reset()
    {
        StartStopTracker.StopFlight();

        _body.position = new Vector3(0, 3, 0);
        _body.rotation = Quaternion.identity;
        _body.velocity = Vector3.zero;
        _body.angularVelocity = Vector3.zero;
    }
}

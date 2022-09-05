using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasHandler : MonoBehaviour
{
    public GameObject CanvasContainer;
    public Canvas Canvas;

    public Toggle Show_1_1;
    public Toggle Show_InFront;

    public GameObject Pedestal;

    public GameObject RigidBodyObject;
    private Rigidbody _body = null;

    public Transform XROrigin;
    public Transform HeadController;

    public FlightStartStopTracker StartStopTracker;

    public HUDFlight HUD;

    private bool _isUIActive = false;

    public void Start()
    {
        _body = RigidBodyObject.GetComponent<Rigidbody>();

        HideCanvas();

        //TODO: load from json
        HUD.ShowVisuals_1_1 = Show_1_1.isOn;
        HUD.ShowVisuals_InFront = Show_InFront.isOn;

        //TODO: update json when there's a change
        Show_1_1.onValueChanged.AddListener(b => HUD.ShowVisuals_1_1 = b);
        Show_InFront.onValueChanged.AddListener(b => HUD.ShowVisuals_InFront = b);
    }

    public void ShowCanvas()
    {
        StartStopTracker.StopFlight();
        _body.velocity = Vector3.zero;
        _body.angularVelocity = Vector3.zero;
        XROrigin.position += new Vector3(0, 12, 0);

        Vector3 standing = new Vector3(HeadController.position.x, HeadController.position.y - 2.5f, HeadController.position.z);
        Vector3 direction = Vector3.ProjectOnPlane(HeadController.forward, Vector3.up).normalized;

        Pedestal.transform.position = standing;

        CanvasContainer.transform.position = standing + (direction * 8) + new Vector3(0, 2, 0);
        CanvasContainer.transform.rotation = Quaternion.FromToRotation(new Vector3(0, 0, 1), direction);

        CanvasContainer.SetActive(true);
        Pedestal.SetActive(true);

        _isUIActive = true;
    }
    public void HideCanvas()
    {
        CanvasContainer.SetActive(false);
        Pedestal.SetActive(false);

        _isUIActive = false;
    }

    private void Update()
    {
        if (!_isUIActive)
            return;

        if ((HeadController.position - Pedestal.transform.position).sqrMagnitude > 4 * 4)
            HideCanvas();
    }
}

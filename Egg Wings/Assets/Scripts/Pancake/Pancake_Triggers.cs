using PerfectlyNormalUnity;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Pancake_Triggers : MonoBehaviour
{
    public float MULT_THRUST = 144;
    public float MULT_BRAKE = 144;

    public GameObject RigidBodyObject;
    private Rigidbody _body = null;

    //public GameObject InfoLabel;
    //private TextMeshProUGUI _info_label;

    private InputXBoxController _input;

    private float _left_trigger = 0f;
    private float _right_trigger = 0f;

    private void Awake()
    {
        _input = new InputXBoxController();
    }

    void Start()
    {
        _body = RigidBodyObject.GetComponent<Rigidbody>();
        //_info_label = InfoLabel.GetComponentInChildren<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        _input.Enable();
    }
    private void OnDisable()
    {
        _input.Disable();
    }

    void Update()
    {
        _left_trigger = _input.Player.TriggerLeft.ReadValue<float>();
        _right_trigger = _input.Player.TriggerRight.ReadValue<float>();

        //_info_label.text = $"left: {_left_trigger.ToStringSignificantDigits(3)}\r\nright: {_right_trigger.ToStringSignificantDigits(3)}";
    }

    private void FixedUpdate()
    {
        if (!_left_trigger.IsNearZero())
            _body.AddRelativeForce(Vector3.up * (-_left_trigger * MULT_BRAKE), ForceMode.Force);

        if(!_right_trigger.IsNearZero())
            _body.AddRelativeForce(Vector3.up * (_right_trigger * MULT_THRUST), ForceMode.Force);
    }
}

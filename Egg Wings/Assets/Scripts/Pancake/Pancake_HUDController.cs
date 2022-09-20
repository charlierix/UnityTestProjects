using PerfectlyNormalUnity;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Pancake_HUDController : MonoBehaviour
{
    public GameObject RigidBodyObject;
    private Rigidbody _body = null;

    public GameObject InfoLabel;
    private TextMeshProUGUI _info_label;

    void Start()
    {
        _body = RigidBodyObject.GetComponent<Rigidbody>();
        _info_label = InfoLabel.GetComponentInChildren<TextMeshProUGUI>();
    }

    void Update()
    {
        _info_label.text = $"velocity: {_body.velocity.ToStringSignificantDigits(0)}\r\nspeed: {_body.velocity.magnitude.ToStringSignificantDigits(0)}";
    }
}

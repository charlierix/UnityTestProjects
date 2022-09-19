using PerfectlyNormalUnity;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Pancake_Thumbstick_Tail : MonoBehaviour
{
    public Transform Target;

    public GameObject InfoLabel;
    private TextMeshProUGUI _info_label;

    private InputXBoxController _input;

    private void Awake()
    {
        _input = new InputXBoxController();
    }

    private void Start()
    {
        _info_label = InfoLabel.GetComponentInChildren<TextMeshProUGUI>();
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
        Vector2 value = _input.Player.ThumbstickRight.ReadValue<Vector2>();

        _info_label.text = value.ToStringSignificantDigits(3);

        float mult = 60;

        Target.rotation = Quaternion.Euler(-value.y * mult, 0, value.x * mult);



    }

    //private static qua

}

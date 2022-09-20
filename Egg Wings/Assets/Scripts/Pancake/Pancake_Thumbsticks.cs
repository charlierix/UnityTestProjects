using PerfectlyNormalUnity;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Pancake_Thumbsticks : MonoBehaviour
{
    public float MULT_TAIL_ELEVATOR = 60;
    public float MULT_TAIL_RUDDER = 60;

    public float MULT_WING_ROOT_STRAFE = 30;
    public float MULT_WING_TIP_STRAFE = 20;

    public float MULT_WING_TIP_ROLL = 45;

    public Transform RigidBody;

    public Transform Target_Wing_Left_Root;
    public Transform Target_Wing_Left_Tip;

    public Transform Target_Wing_Right_Root;
    public Transform Target_Wing_Right_Tip;

    public Transform Target_Tail_Tip;

    //public GameObject InfoLabel;
    //private TextMeshProUGUI _info_label;

    private InputXBoxController _input;

    private void Awake()
    {
        _input = new InputXBoxController();
    }

    private void Start()
    {
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
        Vector2 left = _input.Player.ThumbstickLeft.ReadValue<Vector2>();
        Vector2 right = _input.Player.ThumbstickRight.ReadValue<Vector2>();

        //_info_label.text = $"left: {left.ToStringSignificantDigits(3)}\r\nright: {right.ToStringSignificantDigits(3)}";

        Target_Tail_Tip.rotation = RigidBody.rotation * Quaternion.Euler(-right.y * MULT_TAIL_ELEVATOR, 0, left.x * MULT_TAIL_RUDDER);

        float root_strafe = -left.y * MULT_WING_ROOT_STRAFE;

        Target_Wing_Left_Root.rotation = RigidBody.rotation * Quaternion.Euler(root_strafe, 0, 0);
        Target_Wing_Right_Root.rotation = RigidBody.rotation * Quaternion.Euler(root_strafe, 0, 0);

        float tip_strafe = -left.y * MULT_WING_TIP_STRAFE;
        float tip_roll = right.x * MULT_WING_TIP_ROLL;

        Target_Wing_Left_Tip.rotation = RigidBody.rotation * Quaternion.Euler(tip_strafe - tip_roll, 0, 0);
        Target_Wing_Right_Tip.rotation = RigidBody.rotation * Quaternion.Euler(tip_strafe + tip_roll, 0, 0);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pancake_Triggers : MonoBehaviour
{
    public GameObject RigidBodyObject;
    private Rigidbody _body = null;

    public Engine Engine_Left;
    public Engine Engine_Right;

    private InputXBoxController _input;

    private void Awake()
    {
        _input = new InputXBoxController();
    }

    void Start()
    {
        _body = RigidBodyObject.GetComponent<Rigidbody>();
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
        Engine_Left.Fire_Percent = _input.Player.TriggerLeft.ReadValue<float>();        // this is zero to one
        Engine_Right.Fire_Percent = _input.Player.TriggerRight.ReadValue<float>();
    }
}

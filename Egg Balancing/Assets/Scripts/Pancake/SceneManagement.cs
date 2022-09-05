using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SceneManagement : MonoBehaviour
{
    public GameObject Body;
    private Rigidbody _body = null;

    void Start()
    {
        _body = Body.GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Reset Character
        //if (Input.GetKeyDown(KeyCode.Alpha1))       // old unity input
        if(Keyboard.current.digit1Key.wasPressedThisFrame)      // new unity input
        {
            _body.position = new Vector3(0, 3, 0);
            _body.rotation = Quaternion.identity;
            _body.velocity = Vector3.zero;
            _body.angularVelocity = Vector3.zero;
        }
    }
}

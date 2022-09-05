using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float MouseSensitivity = 300f;
    public bool InvertY = true;

    public Transform PlayerBodyTransform;

    private float _xRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * MouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * MouseSensitivity * Time.deltaTime;

        if(!InvertY)    // positive is invert Y, which is the proper way to look with a mouse
        {
            mouseY = -mouseY;
        }

        _xRotation += mouseY;       
        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
        PlayerBodyTransform.Rotate(Vector3.up * mouseX);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidBodySync : MonoBehaviour
{
    public Transform Head;
    public Transform Foot;

    void Update()
    {
        transform.position = new Vector3(Head.position.x, Foot.position.y, Head.position.z);
    }
}

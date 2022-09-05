using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshTester : MonoBehaviour
{
    void Start()
    {
        //https://catlikecoding.com/unity/tutorials/procedural-meshes/creating-a-mesh/

        // gameobject
        //  meshfilter <-- mesh
        //  meshrenderer <-- material

    }

    private Mesh GetMesh()
    {
        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        mesh.vertices = new[]
        {
            new Vector3(-1, 0.5f, -1),
            new Vector3(1, 0.5f, -1),
            new Vector3(0, 0.5f, 1),
        };

        //mesh.uv = ;

        mesh.triangles = new[]
        {
            0, 1, 2,
        };

        return mesh;
    }

}

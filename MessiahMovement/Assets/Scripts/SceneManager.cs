using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    const float GROUNDY = -.5f;     // ground's y is 1, so place ground tiles at half that

    public Transform PlayerTransform = null;
    public GameObject GroundPrefab = null;
    public GameObject BlockPrefab = null;

    private List<GameObject> _ground = new List<GameObject>();

    private void Start()
    {
        Cursor.visible = false;
        ResetEnvironment_BASIC();
    }

    void Update()
    {
        if(Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))      //NOTE: GetKeyDown only returns true once.  GetKey keeps returning true while they hold the button down
        {

        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
        {
            ResetEnvironment_Blocks();
        }
    }

    private void ResetEnvironment_BASIC()
    {
        PlayerTransform.position = new Vector3(0, 2, 0);

        ClearExisting();

        CreateGround();
    }
    private void ResetEnvironment_Blocks()
    {
        PlayerTransform.position = new Vector3(0, 24, 0);

        ClearExisting();

        CreateGround();

        if (BlockPrefab != null)
        {
            int count = UnityEngine.Random.Range(64, 128);

            for (int cntr = 0; cntr < count; cntr++)
            {
                CreateBlock_Rand();
            }
        }
    }

    private void ClearExisting()
    {
        foreach (var ground in _ground)
        {
            Destroy(ground);
        }
        _ground.Clear();
    }

    private void CreateGround()
    {
        _ground.Add(Instantiate(GroundPrefab, new Vector3(0, -1, 0), Quaternion.identity));
    }
    private void CreateBlock_Rand()
    {
        var pos = new Vector3
        (
            UnityEngine.Random.Range(-60f, 60f),
            UnityEngine.Random.Range(3f, 12f),
            UnityEngine.Random.Range(-60f, 60f)
        );

        GameObject block = Instantiate(BlockPrefab, pos, Quaternion.identity);

        Transform transform = block.GetComponent<Transform>();
        if(transform != null)
        {
            transform.localScale = new Vector3
            (
                UnityEngine.Random.Range(2f, 12f),
                UnityEngine.Random.Range(.5f, 2f),
                UnityEngine.Random.Range(2f, 12f)
            );
        }

        _ground.Add(block);
    }
}

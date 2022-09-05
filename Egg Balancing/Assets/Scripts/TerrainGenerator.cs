using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    public int Height = 60;

    public int Width_Length = 2048;

    public float Scale = 24f;

    public bool ShouldAnimate = true;

    private Terrain _terrain = null;

    private float _offsetX = 100f;
    private float _offsetY = 100f;

    private Vector2 _delta = new Vector2();
    private float _lastDeltaChange = 0f;
    private float _nextChangeTime = 1f;

    private void Start()
    {
        _terrain = GetComponent<Terrain>();

        _offsetX = UnityEngine.Random.Range(0f, 1e5f);
        _offsetY = UnityEngine.Random.Range(0f, 1e5f);
    }

    private void Update()
    {
        if (!ShouldAnimate)
            return;

        _terrain.terrainData = GenerateTerrain(_terrain.terrainData);

        if (_lastDeltaChange > _nextChangeTime)
        {
            _delta = UnityEngine.Random.insideUnitCircle;
            _lastDeltaChange = 0f;
            _nextChangeTime = UnityEngine.Random.Range(.5f, 3f);
        }

        _lastDeltaChange += Time.deltaTime;

        _offsetX += _delta.x * Time.deltaTime;
        _offsetY += _delta.y * Time.deltaTime;
    }

    //Warning: This corrupts basedOn
    private TerrainData GenerateTerrain(TerrainData basedOn)
    {
        basedOn.heightmapResolution = Width_Length + 1;

        basedOn.size = new Vector3(Width_Length, Height, Width_Length);
        basedOn.SetHeights(0, 0, GenerateHeights());

        return basedOn;
    }

    private float[,] GenerateHeights()
    {
        float[,] retVal = new float[Width_Length, Width_Length];

        for (int x = 0; x < Width_Length; x++)
        {
            for (int y = 0; y < Width_Length; y++)
            {
                retVal[x, y] = CalculateHeight(x, y);
            }
        }

        return retVal;
    }

    private float CalculateHeight(int x, int y)
    {
        float xCoord = (float)x / (float)Width_Length * Scale;
        float yCoord = (float)y / (float)Width_Length * Scale;

        xCoord += _offsetX;
        yCoord += _offsetY;

        return Mathf.PerlinNoise(xCoord, yCoord);
    }
}

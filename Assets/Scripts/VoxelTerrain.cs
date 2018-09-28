using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelTerrain : MonoBehaviour {

    [Header("Blocks")]
    public GameObject[] blocks;

    [Header("Size")]
    public float xSize = 100f;
    public float zSize = 100f;

    [Header("Generator Settings")]
    public float frequency = 10f;
    public float amplitude = 10f;

    [Header("Other Settings")]
    public bool enableMinecraft = false;
    
    private Transform _transform;
    private GameObject block;

    private void Start()
    {
        _transform = GetComponent<Transform>();
        GenerateTerrain();
    }

    private void GenerateTerrain()
    {
        for (int x = 0; x < xSize; x++)
        {
            for (int z = 0; z < zSize; z++)
            {
                float y = Mathf.PerlinNoise(x / frequency, z / frequency) * amplitude;
                
                if(enableMinecraft) y = Mathf.Floor(y);

                if (y > 0 && y <= amplitude / 3) block = blocks[0]; // soil
                else if (y > amplitude / 3 && y <= amplitude / 3 * 2) block = blocks[1]; // grass
                else block = blocks[2]; // stone

                GameObject currentBlock = Instantiate(block, new Vector3(x, y, z), Quaternion.identity);
                currentBlock.transform.parent = _transform;
            }
        }
    }

}

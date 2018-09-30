using System;
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
    public Transform bounds;
    public bool enableMinecraft = false;
    
    private Transform _transform;
    private GameObject block;
    private string lastParentName, currentParentName;

    private void Start()
    {
        _transform = GetComponent<Transform>();
        StartCoroutine(GenerateTerrain());
    }

    private IEnumerator GenerateTerrain()
    {
        string guid = Guid.NewGuid().ToString();
        GameObject parent = new GameObject(guid);
        currentParentName = guid;

        Vector3 playerPos = _transform.position;

        for (int x = (int)playerPos.x; x < xSize + (int)playerPos.x; x++)
        {
            for (int z = (int)playerPos.z; z < zSize + (int)playerPos.z; z++)
            {
                float y = Mathf.PerlinNoise(x / frequency, z / frequency) * amplitude;
                
                if(enableMinecraft) y = Mathf.Floor(y);

                if (y > 0 && y <= amplitude / 3) block = blocks[0]; // soil
                else if (y > amplitude / 3 && y <= amplitude / 3 * 2) block = blocks[1]; // grass
                else block = blocks[2]; // stone

                GameObject currentBlock = Instantiate(
                    block, 
                    new Vector3(x - xSize / 2, y, z - zSize / 2), 
                    Quaternion.identity
                );

                currentBlock.transform.parent = parent.transform;
            }
        }


        if (lastParentName != null)
        {
            Destroy(GameObject.Find(lastParentName));
        }

        yield return null;

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Bounds"))
        {
            lastParentName = currentParentName;
            StartCoroutine(GenerateTerrain());
            bounds.transform.position = new Vector3(_transform.position.x, 0f, _transform.position.z);
        }
    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelTerrain : MonoBehaviour {

    public GameObject blankCube;

    [Header("Materials")]
    public Material soil;
    public Material grass;
    public Material stone;

    [Header("Size")]
    public float xSize = 100f;
    public float zSize = 100f;

    [Header("Generator Settings")]
    public float frequency = 10f;
    public float amplitude = 10f;

    [Header("Other Settings")]
    public Transform bounds;
    public bool enableMinecraft = false;

    private GameObject block;
    private Transform _transform;
    private string lastParentName, currentParentName;

    private void Start()
    {
        _transform = GetComponent<Transform>();
        block = CreateCube();
        StartCoroutine(GenerateTerrain());
    }

    private IEnumerator GenerateTerrain()
    {
        string guid = Guid.NewGuid().ToString();
        GameObject parent = new GameObject(guid);
        currentParentName = guid;

        Vector3 playerPos = _transform.position;

        Material blockMaterial;

        for (int x = (int)playerPos.x; x < xSize + (int)playerPos.x; x++)
        {
            for (int z = (int)playerPos.z; z < zSize + (int)playerPos.z; z++)
            {
                float y = Mathf.PerlinNoise(x / frequency, z / frequency) * amplitude;
                
                if(enableMinecraft) y = Mathf.Floor(y);

                if (y > 0 && y <= amplitude / 3) blockMaterial = soil;
                else if (y > amplitude / 3 && y <= amplitude / 3 * 2) blockMaterial = grass;
                else blockMaterial = stone;

                GameObject currentBlock = Instantiate(
                    block, 
                    new Vector3(x - xSize / 2, y, z - zSize / 2), 
                    Quaternion.identity
                );

                currentBlock.GetComponent<Renderer>().material = blockMaterial;

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

    private GameObject CreateCube()
    {
        GameObject cube = blankCube;

        Vector3[] vertices = {
            new Vector3 (0, 0, 0),
            new Vector3 (1, 0, 0),
            new Vector3 (1, 1, 0),
            new Vector3 (0, 1, 0),
            new Vector3 (0, 1, 1),
            new Vector3 (1, 1, 1),
            new Vector3 (1, 0, 1),
            new Vector3 (0, 0, 1),
        };

        int[] triangles = {
            0, 2, 1, 
			0, 3, 2,
            2, 3, 4, 
			2, 4, 5,
            1, 2, 5, 
			1, 5, 6,
            0, 7, 4, 
			0, 4, 3,
            5, 4, 7, 
			5, 7, 6,
            0, 6, 7, 
			0, 1, 6
        };

        MeshFilter mr = cube.GetComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mr.mesh = mesh;

        return cube;
    }

}

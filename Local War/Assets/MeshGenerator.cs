using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MeshGenerator : MonoBehaviour
{

    // Constant variables, edit to change effects

    // City Map stuff
    const int citySize = 3;                                        // Number of blocks in city
    const int blockSize = 5;                                        // Number of building locations per block
    const int buildMaxSize = 3;                                     // Max size of a building in X or Z dimension

    // Spacing for buildings
    const float buildingSpacing = .2f;                              // Empty space around buildings, min 0 max .49
    const float buildingThickness = .05f;                           // Wall thickness

    // Building height stuff
    const float baseHeight = 2f;                                    // Mininum building height
    const float baseRange = 3f;                                     // Range above minimum building height
    const float extraTallBuildings = 0.01f;                         // Adds a % chance of a building to be extraHeight taller
    const float extraHeight = 10.0f;                                // Extra height in case of building being taller

    // Building / Ground Noise
    bool groundNoise = false;                                        // Change to add noise to ground level, also raises ground slightly, not reccommended but fun
    bool buildingHeightNoise = false;                               // Adds noise to building height, also increases building height, dependent on citySize

    // Derived constants, do not edit
    const int xSize = blockSize * citySize + citySize - 1;          // Mesh size in X
    const int zSize = blockSize * citySize + citySize - 1;          // Mesh size in Y
    const int mapScale = 1;                                         // This is not implemented correctly yet and can break certain things if not 1
    const float buildN = ((float)citySize * (float)citySize * (float)blockSize) / (2.0f * (float)citySize + (float)blockSize);

    // Other variables
    Mesh mesh;
    Vector3[] vertices;
    int[] triangles;
    Vector2[] uvs;
    int[] block;

    // Use this for initialization
    void Start()
    {

        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        // Create mesh
        CreateShape();
        UpdateMesh();

        // Generate blocks
        for (int i = 0; i < citySize; i++)
        {
            for (int k = 0; k < citySize; k++)
            {
                BuildingGeneration((blockSize * i) + i, (blockSize * k) + k);
            }
        }
    }

    void BuildingGeneration(int x, int z)
    {
        // Convert ints to floats for calculation
        float xStart = x;
        float zStart = z;
        int[] buildingSize = {
            Random.Range(1,buildMaxSize+1),
            Random.Range(1,buildMaxSize+1)
        };

        // For debugging only, can delete later
        int buildingCounter = 0;

        block = new int[blockSize * blockSize];

        // Set every space in the block as free
        for (int i = 0, row = 0; i < blockSize; i++)
        {
            for (int k = 0; k < blockSize; k++)
            {
                block[k + row * blockSize] = 0;
            }
            row++;
        }

        // Generate buildings
        for (int i = 0, row = 0; i < blockSize; i++)
        {
            for (int k = 0; k < blockSize; k++)
            {
                // For every square in the block, as long as that square is free
                if (block[k + row * blockSize] == 0)
                {
                    // For debugging only, can delete later
                    buildingCounter++;
                    // Debug.Log("Building " + buildingCounter + ": " + buildingSize[0] + "x" + buildingSize[1]);

                    // Get necessary vairable points, this could be cleaned up a lot
                    float x1 = xStart + i + buildingSpacing;
                    float x2 = Mathf.Min((xStart + i + buildingSize[0] - buildingSpacing), (xStart + blockSize - buildingSpacing));
                    float xMid = (x1 + x2) / 2;
                    float z1 = zStart + k + buildingSpacing;
                    float z2 = Mathf.Min((zStart + k + buildingSize[1] - buildingSpacing), (zStart + blockSize - buildingSpacing));
                    float zMid = (z1 + z2) / 2;
                    float buildingHeight = Random.Range(baseHeight, baseHeight + baseRange);
                    if (buildingHeightNoise == true)
                    {
                        buildingHeight += Mathf.PerlinNoise(x1 * 1.0f / buildN, z1 * 1.0f / buildN) * buildN;
                    }
                    float tallB = Random.Range(0f, 1f);
                    if (tallB < extraTallBuildings)
                    {
                        buildingHeight += extraHeight;
                    }
                    float yMid = buildingHeight / 2;
                    float buildingX = x2 - x1;
                    float buildingZ = z2 - z1;

                    // Debug.Log("x1: " + x1 + " x2:" + x2 + " z1:" + z1 + " z2:" + z2);

                    // Set squares to used
                    for (int j = 0; j < buildingX; j++)
                    {
                        for (int r = 0; r < buildingZ; r++)
                        {
                            // Debug.Log(buildingCounter + ": " + (k + j + ((row + r) * blockSize)));
                            block[k + r + ((row + j) * blockSize)] = 1;
                        }
                    }

                    // Build Walls
                    BuildWall(xMid, yMid, z1, buildingX, buildingHeight, buildingThickness);
                    BuildWall(xMid, yMid, z2, buildingX, buildingHeight, buildingThickness);
                    BuildWall(x1, yMid, zMid, buildingThickness, buildingHeight, buildingZ);
                    BuildWall(x2, yMid, zMid, buildingThickness, buildingHeight, buildingZ);
                    BuildWall(xMid, buildingHeight, zMid, buildingX, buildingThickness, buildingZ);

                    // Randomize next building size
                    buildingSize[0] = Random.Range(1, buildMaxSize + 1);
                    buildingSize[1] = Random.Range(1, buildMaxSize + 1);

                    // Extra randomize logic to avoid min or max size buildings as much as possible
                    if (buildingSize[1] == 1 && buildingSize[0] == 1 || buildingSize[1] == buildMaxSize && buildingSize[0] == buildMaxSize)
                    {
                        buildingSize[1] = Random.Range(1, buildMaxSize + 1);
                        buildingSize[0] = Random.Range(1, buildMaxSize + 1);
                    }
                    if ((buildingSize[0] + i) >= blockSize)
                    {
                        buildingSize[0] = 1;
                    }
                    if ((buildingSize[1] + k) >= blockSize)
                    {
                        buildingSize[1] = 1;
                    }
                }
            }
            row++;
        }

    }

    // Creates a wall at (x, y, z) position with (x, y, z) scale.
    void BuildWall(float px, float py, float pz, float sx, float sy, float sz)
    {
        Material newMat = Resources.Load("Art/Material/Pure Color/Green", typeof(Material)) as Material;
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = new Vector3(px, py, pz);
        cube.transform.localScale = new Vector3(sx, sy, sz);
        cube.GetComponent<MeshRenderer>().material = newMat;
    }

    void CreateShape()
    {
        // Create Surface Vertices
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];

        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float y;

                if (groundNoise)
                {
                    y = Mathf.PerlinNoise(x * 1.0f / buildN, z * 1.0f / buildN) * buildN - 1;
                }
                else
                {
                    y = 0;
                }

                vertices[i] = new Vector3(x, y, z);
                i++;
            }
        }

        // Create Surface Triangles
        triangles = new int[xSize * zSize * 6];
        int vert = 0;
        int tris = 0;

        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                triangles[tris + 0] = (vert + 0) * mapScale;
                triangles[tris + 1] = (vert + xSize + 1) * mapScale;
                triangles[tris + 2] = (vert + 1) * mapScale;
                triangles[tris + 3] = (vert + 1) * mapScale;
                triangles[tris + 4] = (vert + xSize + 1) * mapScale;
                triangles[tris + 5] = (vert + xSize + 2) * mapScale;

                vert++;
                tris += 6;
            }
            vert++;
        }

        // Create Texture Map
        uvs = new Vector2[vertices.Length];

        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                uvs[i] = new Vector2((float)x / xSize, (float)z / zSize);
                i++;
            }
        }
    }

    void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;

        mesh.RecalculateNormals();
        MeshCollider meshc = gameObject.AddComponent(typeof(MeshCollider)) as MeshCollider;
        meshc.sharedMesh = mesh;
    }

}


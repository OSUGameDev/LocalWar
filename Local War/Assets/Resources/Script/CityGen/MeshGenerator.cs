using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MeshGenerator : NetworkBehaviour
{

    // Constant variables, edit to change effects

    // City Map stuff
    const int citySize = 10;                                         // Number of blocks in city
    const int blockSize = 10;                                       // Number of building locations per block
    const int buildMinSize = 3;                                     // Min size of a building in X and Z dimension
    const int buildMaxSize = 5;                                     // Max size of a building in X and Z dimension
    const int roadSize = 2;                                         // Size of Roads
    const float buildingSpacing = .4f;                              // Empty space around buildings, min 0 max .49
    const float buildingThickness = .05f;                           // Wall thickness

    // Building height stuff
    const float baseHeight = 1.5f;                                    // Mininum building height
    const float baseRange = 3.5f;                                     // Range above minimum building height
    const float extraTallBuildings = 0.02f;                         // Adds a % chance of a building to be extraHeight taller
    const float extraHeight = 10.0f;                                // Extra height in case of building being taller

    // Building / Ground Noise
    bool groundNoise = true;                                        // Change to add noise to ground level, also raises ground slightly, not reccommended but fun
    bool buildingHeightNoise = true;                                // Adds noise to building height, also increases building height, dependent on citySize

    // Perlin noise changes
    readonly float refinement = 0.01f;                              // Changes the frequency of the noise, lower number = smoother terrain
    readonly float magnitude = 45f;                                 // Changes the magnitude, lower numbers = flatter terrain

    // Random seed for level generation
    [SyncVar]
    public int seed = 1;

    // Derived constants, DO NOT EDIT
    const int xSize = (blockSize * citySize) + (citySize * roadSize) + (roadSize);          // Mesh size in X
    const int zSize = (blockSize * citySize) + (citySize * roadSize) + (roadSize);          // Mesh size in Y
    float x_offset;
    float z_offset;

    // Other variables
    Mesh mesh;
    Vector3[] vertices;
    int[] triangles;
    Vector2[] uvs;
    int[] block;

    // Use this for initialization
    void Start()
    {
        if (isServer)
            this.seed = Random.Range(int.MinValue, int.MaxValue);
        GenerateMap();

    }


    public void GenerateMap()
    {
        Random.InitState(seed);
        Debug.Log(seed);
        x_offset = Random.Range(-100, 100);
        z_offset = Random.Range(-100, 100);

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
                BuildingGeneration((blockSize * i) + (i * roadSize) + roadSize, (blockSize * k) + (k * roadSize) + roadSize);
            }
        }

        // Generate Invisible Arena Walls
        InvisibleWall();
    }

    void BuildingGeneration(int x, int z)
    {
        // Convert ints to floats for calculation
        float xStart = x;
        float zStart = z;
        int[] buildingSize = {
            Random.Range(buildMinSize,buildMaxSize+1),
            Random.Range(buildMinSize,buildMaxSize+1)
        };

        // define block object for building placement logic
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
                    // Randomize next building size
                    buildingSize[0] = Random.Range(buildMinSize, buildMaxSize + 1);
                    buildingSize[1] = Random.Range(buildMinSize, buildMaxSize + 1);

                    // Extra randomize logic
                    for (int r = buildMinSize - 1; r > 0; r--)
                    {
                        if ((buildingSize[0] + i + r) == blockSize)
                        {
                            buildingSize[0] += r;
                        }
                        if ((buildingSize[1] + k + r) == blockSize)
                        {
                            buildingSize[1] += r;
                        }
                    }

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
                        buildingHeight += Mathf.PerlinNoise((x1 + x_offset) * refinement, (z1 + z_offset) * refinement) * magnitude;
                    }
                    float tallB = Random.Range(0f, 1f);
                    if (tallB < extraTallBuildings)
                    {
                        buildingHeight += extraHeight;
                    }
                    float yMid = buildingHeight / 2;
                    float buildingX = x2 - x1;
                    float buildingZ = z2 - z1;

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
                    BuildCeil(xMid, buildingHeight, zMid, buildingX, buildingThickness, buildingZ);
                }
            }
            row++;
        }

    }

    // Creates a wall at (x, y, z) position with (x, y, z) scale.
    void BuildWall(float px, float py, float pz, float sx, float sy, float sz)
    {
        Material newMat = Resources.Load("Art/Material/Brick", typeof(Material)) as Material;
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = new Vector3(px, py, pz);
        cube.transform.localScale = new Vector3(sx, sy, sz);
        cube.GetComponent<MeshRenderer>().material = newMat;
        cube.AddComponent<ReCalcCubeTexture>();
		float color = Random.Range (0.0f, 0.5f);
		Color teal = new Vector4 (0, .5f+color, .5f+color);
		Color yellow = new Vector4 (0, .5f+color, color);
		Color red = new Vector4 (0, color, .5f+color);
		int choice = Random.Range (0, 10);
		var cubeRenderer = cube.GetComponent<Renderer>();
		if (choice <= 1) 
		{
			cubeRenderer.material.SetColor("_Color", yellow);
		}
		else if (choice >= 7) 
		{
			cubeRenderer.material.SetColor("_Color", red);
		}
		else 
		{
			cubeRenderer.material.SetColor("_Color", teal);
		}
    }

	void BuildCeil(float px, float py, float pz, float sx, float sy, float sz)
	{
		Material newMat = Resources.Load("Art/Material/Brick", typeof(Material)) as Material;
		GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
		cube.transform.position = new Vector3(px, py, pz);
		cube.transform.localScale = new Vector3(sx, sy, sz);
		cube.GetComponent<MeshRenderer>().material = newMat;
		cube.AddComponent<ReCalcCubeTexture>();
	}

	void BuildInvisWall(float px, float py, float pz, float sx, float sy, float sz)
	{
		Material newMat = Resources.Load("Art/Material/Pure Color/Clear", typeof(Material)) as Material;
		GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
		cube.transform.position = new Vector3(px, py, pz);
		cube.transform.localScale = new Vector3(sx, sy, sz);
		var cubeRenderer = cube.GetComponent<Renderer>();
		cubeRenderer.enabled = false;
	}

    // Create Invisible walls so players can't fall off the plane
    void InvisibleWall()
    {
        //Total arena size per wall
        float arenaWallSize = (citySize * blockSize + citySize * roadSize + roadSize);

        //Height of wall
        float arenaWallHeight = 100f;

        // Width of wall
        float arenaWallWidth = .5f;

        // Build Bottom Left Walls
		BuildInvisWall(arenaWallSize / 2, arenaWallHeight/2, 0, arenaWallSize, arenaWallHeight, arenaWallWidth);
		BuildInvisWall(0, arenaWallHeight/2, arenaWallSize / 2, arenaWallWidth, arenaWallHeight, arenaWallSize);

        // Build Top Right Walls
		BuildInvisWall(arenaWallSize, arenaWallHeight/2, arenaWallSize / 2, arenaWallWidth, arenaWallHeight, arenaWallSize);
		BuildInvisWall(arenaWallSize / 2, arenaWallHeight/2, arenaWallSize, arenaWallSize, arenaWallHeight, arenaWallWidth);
		BuildInvisWall(arenaWallSize / 2, arenaWallHeight, arenaWallSize/2, arenaWallSize, arenaWallWidth, arenaWallSize);
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
                    y = Mathf.PerlinNoise((x + x_offset) * refinement, (z + z_offset) * refinement) * magnitude - 1;
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
                triangles[tris + 0] = (vert + 0);
                triangles[tris + 1] = (vert + xSize + 1);
                triangles[tris + 2] = (vert + 1);
                triangles[tris + 3] = (vert + 1);
                triangles[tris + 4] = (vert + xSize + 1);
                triangles[tris + 5] = (vert + xSize + 2);

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


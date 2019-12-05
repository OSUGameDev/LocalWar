using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;

public class MeshGenerator : NetworkBehaviour
{
    // Random seed for level generation
    [SyncVar]
    public int seed = 1;
    float x_offset;
    float z_offset;

    // Other variables
    Mesh mesh;
    Vector3[] vertices;
    int[] triangles;
    Vector2[] uvs;
    int[] block;

    MapGenSettings mapGenSettings;

    // Use this for initialization
    void Start()
    {
        loadSettinsFile();
        GenerateMap();
    }

    public void GenerateMap()
    {
        Random.InitState(seed);
        x_offset = Random.Range(-100, 100);
        z_offset = Random.Range(-100, 100);

        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        // Create mesh
        CreateShape();
        UpdateMesh();

        // Generate blocks
        for (int i = 0; i < mapGenSettings.citySize; i++)
        {
            for (int k = 0; k < mapGenSettings.citySize; k++)
            {
                BuildingGeneration((mapGenSettings.blockSize * i) + (i * mapGenSettings.roadSize) + mapGenSettings.roadSize,
                    (mapGenSettings.blockSize * k) + (k * mapGenSettings.roadSize) + mapGenSettings.roadSize);
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
            Random.Range(mapGenSettings.buildMinSize,mapGenSettings.buildMaxSize+1),
            Random.Range(mapGenSettings.buildMinSize,mapGenSettings.buildMaxSize+1)
        };

        // define block object for building placement logic
        block = new int[mapGenSettings.blockSize * mapGenSettings.blockSize];

        // Set every space in the block as free
        for (int i = 0, row = 0; i < mapGenSettings.blockSize; i++)
        {
            for (int k = 0; k < mapGenSettings.blockSize; k++)
            {
                block[k + row * mapGenSettings.blockSize] = 0;
            }
            row++;
        }

        // Generate buildings
        for (int i = 0, row = 0; i < mapGenSettings.blockSize; i++)
        {
            for (int k = 0; k < mapGenSettings.blockSize; k++)
            {
                // For every square in the block, as long as that square is free
				if (block[k + row * mapGenSettings.blockSize] == 0)
                {
                    // Randomize next building size
                    buildingSize[0] = Random.Range(mapGenSettings.buildMinSize, mapGenSettings.buildMaxSize + 1);
                    buildingSize[1] = Random.Range(mapGenSettings.buildMinSize, mapGenSettings.buildMaxSize + 1);

                    // Extra randomize logic
                    for (int r = mapGenSettings.buildMinSize; r > 0; r--)
                    {
                        if ((buildingSize[0] + i + r) == mapGenSettings.blockSize)
                        {
                            buildingSize[0] += r;
                        }
                        if ((buildingSize[1] + k + r) == mapGenSettings.blockSize)
                        {
                            buildingSize[1] += r;
                        }
                    }

					if (k + buildingSize [1] > mapGenSettings.blockSize) {
						buildingSize [1] = mapGenSettings.blockSize - k;
					}

					if (row + buildingSize [0] > mapGenSettings.blockSize) {
						buildingSize [0] = mapGenSettings.blockSize - row;
					}

					// Prevent overlap
					Debug.Log("k: " + k + " row: " + row + " bs0:" + buildingSize[0] + " bs1:" + buildingSize[1] + " logicX: " + (k + (row + buildingSize[0] - 1) * mapGenSettings.blockSize) + " LogicZ: " + (k + buildingSize[1] - 1 + row * mapGenSettings.blockSize));

					for (int r = buildingSize[0] - 1; r > 0; r--) 
					{
						if (block [k + (r + row) * mapGenSettings.blockSize] == 1) 
						{
							buildingSize [0] = buildingSize [0] - 1;
						}
					}

					for (int r = buildingSize[1] - 1; r > 0; r--) 
					{
						if (block [k + buildingSize[1] - 1 + row * mapGenSettings.blockSize] == 1) 
						{
							buildingSize [1] = buildingSize [1] - 1;
						}
					}

                    // Get necessary vairable points, this could be cleaned up a lot
                    float x1 = xStart + i + mapGenSettings.buildingSpacing;
                    float x2 = Mathf.Min((xStart + i + buildingSize[0] - mapGenSettings.buildingSpacing),
                        (xStart + mapGenSettings.blockSize - mapGenSettings.buildingSpacing));
                    float xMid = (x1 + x2) / 2;
                    float z1 = zStart + k + mapGenSettings.buildingSpacing;
                    float z2 = Mathf.Min((zStart + k + buildingSize[1] - mapGenSettings.buildingSpacing),
                        (zStart + mapGenSettings.blockSize - mapGenSettings.buildingSpacing));
                    float zMid = (z1 + z2) / 2;
                    float buildingHeight = Random.Range(mapGenSettings.baseHeight, mapGenSettings.baseHeight + mapGenSettings.baseRange);
                    if (mapGenSettings.buildingHeightNoise == true)
                    {
                        buildingHeight += Mathf.PerlinNoise((x1 + x_offset) * mapGenSettings.refinement,
                            (z1 + z_offset) * mapGenSettings.refinement) * mapGenSettings.magnitude;
                    }
                    float tallB = Random.Range(0f, 1f);
                    if (tallB < mapGenSettings.extraTallBuildings)
                    {
                        buildingHeight += mapGenSettings.extraHeight;
                    }
                    float yMid = buildingHeight / 2;
                    float buildingX = x2 - x1;
                    float buildingZ = z2 - z1;

                    // Set squares to used
                    for (int j = 0; j < buildingX; j++)
                    {
                        for (int r = 0; r < buildingZ; r++)
                        {
                            block[k + r + ((row + j) * mapGenSettings.blockSize)] = 1;
                        }
                    }

                    // Build Walls
					if (buildingSize [0] >= mapGenSettings.buildMinSize && buildingSize [1] >= mapGenSettings.buildMinSize) 
					{
	                    BuildWall(xMid, yMid, z1, buildingX, buildingHeight,mapGenSettings.buildingThickness);
	                    BuildWall(xMid, yMid, z2, buildingX, buildingHeight, mapGenSettings.buildingThickness);
	                    BuildWall(x1, yMid, zMid, mapGenSettings.buildingThickness, buildingHeight, buildingZ);
	                    BuildWall(x2, yMid, zMid, mapGenSettings.buildingThickness, buildingHeight, buildingZ);
	                    BuildCeil(xMid, buildingHeight, zMid, buildingX, mapGenSettings.buildingThickness, buildingZ);
					}
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
        float arenaWallSize = 
            (mapGenSettings.citySize * mapGenSettings.blockSize 
            + mapGenSettings.citySize * mapGenSettings.roadSize 
            + mapGenSettings.roadSize);

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
        vertices = new Vector3[(mapGenSettings.xSize + 1) * (mapGenSettings.zSize + 1)];

        for (int i = 0, z = 0; z <= mapGenSettings.zSize; z++)
        {
            for (int x = 0; x <= mapGenSettings.xSize; x++)
            {
                float y;

                if (mapGenSettings.groundNoise)
                {
                    y = Mathf.PerlinNoise((x + x_offset) * mapGenSettings.refinement,
                        (z + z_offset) * mapGenSettings.refinement) * mapGenSettings.magnitude - 1;
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
        triangles = new int[mapGenSettings.xSize * mapGenSettings.zSize * 6];
        int vert = 0;
        int tris = 0;

        for (int z = 0; z < mapGenSettings.zSize; z++)
        {
            for (int x = 0; x < mapGenSettings.xSize; x++)
            {
                triangles[tris + 0] = (vert + 0);
                triangles[tris + 1] = (vert + mapGenSettings.xSize + 1);
                triangles[tris + 2] = (vert + 1);
                triangles[tris + 3] = (vert + 1);
                triangles[tris + 4] = (vert + mapGenSettings.xSize + 1);
                triangles[tris + 5] = (vert + mapGenSettings.xSize + 2);

                vert++;
                tris += 6;
            }
            vert++;
        }

        // Create Texture Map
        uvs = new Vector2[vertices.Length];

        for (int i = 0, z = 0; z <= mapGenSettings.zSize; z++)
        {
            for (int x = 0; x <= mapGenSettings.xSize; x++)
            {
                uvs[i] = new Vector2((float)x / mapGenSettings.xSize, (float)z / mapGenSettings.zSize);
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

    public void loadSettinsFile()
    {
        if (!isServer)
            return;
        seed = Random.Range(int.MinValue, int.MaxValue);
        if (File.Exists("MapGenerationFile.xml"))
            try
            {
                using (var fp = File.OpenRead("MapGenerationFile.xml"))
                    mapGenSettings = CustomSerializer.Deserialize<MapGenSettings>(fp);
                return;
            }
            catch
            {
                Debug.LogError("Unable to read map generation settings file");
                File.Move("MapGenerationFile.xml", "CorruptMapGenerationFile.xml");
            }
        mapGenSettings = new MapGenSettings();
        using (var fp = File.Create("MapGenerationFile.xml"))
            CustomSerializer.Serialize(new MapGenSettings(), fp);

    }

    public override void OnDeserialize(NetworkReader reader, System.Boolean initialState)
    {
        if (initialState == false)
            return;
        seed = reader.ReadInt32();
        mapGenSettings = CustomSerializer.Deserialize<MapGenSettings>(reader);
    }

    public override System.Boolean OnSerialize(NetworkWriter writer, System.Boolean initialState)
    {
        if (initialState != true)
            return false;
        if (mapGenSettings == null)
            loadSettinsFile();
        writer.Write(seed);
        CustomSerializer.Serialize(mapGenSettings, writer);
        return true;
    }
}


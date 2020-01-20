using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiatePrefab : MonoBehaviour
{
    public int height = 4;
    public int length = 20;
    public int width = 20;

    public int test;
    public Quaternion rot;
    public GameObject empty;
    public GameObject solid;
    public GameObject floor;
    public GameObject wall;
    public GameObject corner;

    void Start()
    {
        for (int y = 0; y < height; y++)
        {
            for (int z = 0; z < length; z++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (y == 0)
                    {
                        test = Random.Range(2, 5);
                    } 

                    else if (y == height - 1)
                    {
                        test = Random.Range(1, 3);
                    }

                    else
                    {
                        test = Random.Range(1, 5);
                    }

                    rot = Quaternion.Euler(0, Random.Range(0, 3) * 90, 0);

                    switch (test)
                    {
                        case 0:
                            Instantiate(empty, new Vector3(x * 3, y * 3, z * 3), rot);
                            break;
                        case 1:
                            Instantiate(solid, new Vector3(x * 3, y * 3, z * 3), rot);
                            break;
                        case 2:
                            Instantiate(floor, new Vector3(x * 3, y * 3, z * 3), rot);
                            break;
                        case 3:
                            Instantiate(wall, new Vector3(x * 3, y * 3, z * 3), rot);
                            break;
                        case 4:
                            Instantiate(corner, new Vector3(x * 3, y * 3, z * 3), rot);
                            break;
                    }
                }
            }
        }
        GameObject spawn = GameObject.Find("PlayerSpawnPoint");
        Vector3 minusy = new Vector3(0, 5, 0);
        Debug.Log(spawn.transform.position - minusy);
        Instantiate(floor, spawn.transform.position - minusy, Quaternion.identity);
    }
}

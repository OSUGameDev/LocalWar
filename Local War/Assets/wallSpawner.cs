using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wallSpawner : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject walls;
    void Start()
    {
        spawnFirstWallSet();
        spawnSecondWallSet();
        spawnLastWall();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void spawnFirstWallSet()
    {
        int x = -20; //increment in 5 go -10, -5, skip 0, 5, 10
        int y = 0; //keep y as zero
        int z = 8; //increment after each set of walls so, 8 to start

        Quaternion spawnRotation = Quaternion.identity;

        while (x <= 20)
        {
            if (x != -20)
            {

                Vector3 spawnPosition = new Vector3(x, y, z); //get spawnposition
                Instantiate(walls, spawnPosition, spawnRotation); //instantiate a new game object 
            }
            x += 10;
        }
    }

    void spawnSecondWallSet()
    {
        int x = -15; //increment in 10 such as -15->-5->5-->15
        int y = 0; //keep y as zero
        int z = 14; //increment after each set of walls so, 8 to start

        Quaternion spawnRotation = Quaternion.identity;

        while (x <= 15)
        {

            Vector3 spawnPosition = new Vector3(x, y, z); //get spawnposition
            Instantiate(walls, spawnPosition, spawnRotation); //instantiate a new object at that spawn location

            x += 10; //increase our x position by 10
        }
    }

    void spawnLastWall()
    {
        int x = -10; //increment in 5 go -10, -5, skip 0, 5, 10
        int y = 0; //keep y as zero
        int z = 20; //increment after each set of walls so, 8 to start

        Quaternion spawnRotation = Quaternion.identity; //get spawn rotation need to instantiate

        Vector3 spawnPosition = new Vector3(x, y, z); //get spawnposition
        Instantiate(walls, spawnPosition, spawnRotation);

        x = 10; //just two walls so no loop
        spawnPosition = new Vector3(x, y, z); //get spawnposition
        Instantiate(walls, spawnPosition, spawnRotation);
    }
}

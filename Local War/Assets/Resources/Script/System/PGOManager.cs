using System.Collections.Generic;
using UnityEngine;
using System;

public class PGOManager : MonoBehaviour
{

    [SerializeField]
    private bool canGrow = true;            //if the number of objects in the pool can grow or not

    [SerializeField]
    private int pooledAmount = 10;          //the number of objects in the pool

    //contains pool of objects to grab from 
    private Dictionary<string, List<GameObject>> pooledObjectsHT;

    //used to grow the pool with a clean object (not used repeatdly)
    private Dictionary<string, GameObject> pristeneObjects;  

    // Use this for initialization
    public void Awake() //must be awake because guns were initializing prior to this function call.
    { 
        //initialize the pooled objects, list of lists.
        pooledObjectsHT = new Dictionary<string, List<GameObject>>();    
        pristeneObjects = new Dictionary<string, GameObject>();
    }

    /// <summary>
    /// when called, returns an object in the pool that is currently free to use.
    /// It does this by looping through the pool (list) and returning the first object
    /// it finds that is currently deactivated. If there are none and the pool can grow,
    /// it creates a new object, adds it to the pool, and returns that object.
    /// This method is to be used by other scripts in the scene.
    /// </summary>
    /// <returns>GameObject Pooled Object</returns>
    public GameObject GetPooledObject(string objectKey)
    {
        if (objectKey == null || objectKey.Length == 0)
        {
            //TODO: Debatable whether this debug statement is needed. Has the potential to be called thousands of times and flood the debug log. 
            Debug.LogError("BAD OBJECT ID IN PooledGameObjects: " + objectKey + "\nMaybe you didn't initialize the object?");
            return null;
        }

        //TODO: Make finding an inactive object faster. O(n) currently. 
        List<GameObject> pooledObjects = (List<GameObject>) pooledObjectsHT[objectKey];
        for (int i = 0; i < pooledObjects.Count; i++) //loop through the pool
        {
            if (!(pooledObjects[i].activeInHierarchy))    //if we find an inactive object, return it.
            {
                return pooledObjects[i];
            }
        }


        //if we reach this statement, then all objects are currently active
        if (canGrow)            //if no inactive object was found and the pool can grow, create one and return it.
        {
            GameObject obj = Instantiate((GameObject)pristeneObjects[objectKey]);
            pooledObjects.Add(obj);
            return obj;
        }

        return null;
    }

    /// <summary>
    /// Initializes the object with a specific key.
    /// </summary>
    /// <returns> ObjectId that will be used get a pooled object. </returns>
    public Type InitObject(GameObject pooledObj)
    {
        Guid t = pooledObj.GetType().GUID;
        Debug.Log("GUID:" + t);

// //pristeneObjects.Add(pooledObj);
// //pooledObjectsHT.Add(pooledObj.GetType() ,new List<GameObject>());
// //for (int i = 0; i < pooledAmount; i++)      //this loop creates the pooled objects and adds them to the pool (List) and deactivates them.
// {
//     GameObject obj = (GameObject)Instantiate(pooledObj);
//     obj.SetActive(false);

//     //pooledObjectsHT[pooledObjectsHT.Count - 1].Add(obj);
// }

        return pooledObj.GetType(); //id is the current size of the list-1
    }

    public override string ToString()
    {
        int tObjects = 0;
        string str = "PooledObjects: " + pooledObjectsHT.Count + ", ObjectNames: [";
        string[] keys = new string[pooledObjectsHT.Keys.Count];
        pooledObjectsHT.Keys.CopyTo(keys, 0);

        for (int x = 0; x < keys.Length; x++)
        {
            List<GameObject> pooledObjects = pooledObjectsHT[keys[x]];

            char seperator = ',';
            if (x == pooledObjects.Count - 1)
            {
                seperator = ']';
            }


            
            if (pooledObjects.Count > 0)
            {
                str += pooledObjects[0].name + seperator;
            }

            tObjects += pooledObjects.Count;
        }
        if (pooledObjectsHT.Count == 0) { str += ']'; } //closing if empty


        str += "\nCanGrow:" + canGrow + ", InitialPooledObjects:" + pooledAmount + "TotalPooledObjects:" + tObjects;


        return str;
    }
}
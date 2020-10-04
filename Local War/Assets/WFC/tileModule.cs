using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tileModule : MonoBehaviour
{
    public GameObject POSITIVE_X_TILE;
    public GameObject NEGATIVE_X_TILE;
    public GameObject POSITIVE_Y_TILE;
    public GameObject NEGATIVE_Y_TILE;
    public GameObject POSITIVE_Z_TILE;
    public GameObject NEGATIVE_Z_TILE;

    public int POSITIVE_X_ROTATION;
    public int NEGATIVE_X_ROTATION;
    public int POSITIVE_Y_ROTATION;
    public int NEGATIVE_Y_ROTATION;
    public int POSITIVE_Z_ROTATION;
    public int NEGATIVE_Z_ROTATION;

    public bool Rotatable;
    public int Frequency;
    public int MaxEntropyThreshhold;
    public bool AllowOnEdges;
    public bool AllowOnCeiling;
    public bool AllowOnFloor;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

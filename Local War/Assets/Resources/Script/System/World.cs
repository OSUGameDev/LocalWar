using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour {

    GameObject player;

    // Use this for initialization
    void Start () {
		player = Resources.Load<GameObject>("Prefab/Character/Player");
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}

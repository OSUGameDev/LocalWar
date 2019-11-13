using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PGOTester : MonoBehaviour {

	public GameObject testObj;
	protected PGOManager pgoManager;

	// Use this for initialization
	void Start () {
		pgoManager = GetComponent<PGOManager>();
		Debug.Log(pgoManager);
		Type key = pgoManager.InitObject(testObj);

		Debug.Log("Type:" + key.ToString());
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButton("Fire1"))
        {
            

        }
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class PGOTester : MonoBehaviour {

	public GameObject testObj;
	protected PGOManager pgoManager;
	//this is an example of how to implement PooledGameObjects in a class
	// Use this for initialization
	void Start () {
		Stopwatch timer = new Stopwatch();

		timer.Start();
		//NOTE: EventSystem is the object that contains the PGOManager as a component. If the parent class changes, rename EventSystem to whatever the name 
				//of the parent in the scene is. 

		pgoManager = GameObject.Find("EventSystem").GetComponent<PGOManager>();
		UnityEngine.Debug.Log(pgoManager);
		bool key = pgoManager.InitObject(GetType(), testObj);

		

		timer.Stop();
		UnityEngine.Debug.Log("PGO Loadtime: " + timer.ElapsedMilliseconds + "ms");
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButton("Fire1"))
        {
            GameObject g = pgoManager.GetPooledObject(this.GetType());
			//RESET ALL VARIABLES HERE (timers, position, states, etc...).
			g.transform.position = this.gameObject.transform.position;
			g.SetActive(true);
        }
	}
}

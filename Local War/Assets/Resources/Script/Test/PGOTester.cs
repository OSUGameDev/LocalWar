using UnityEngine;
using System.Diagnostics;
using System;

public class PGOTester : MonoBehaviour {

	//<summary>Toggle this to compare the time needed to instantiate an object vs using a preexsting one</summary>
	public bool disablePGO = false;

	public GameObject testObj;
	protected PGOManager pgoManager;

	private Stopwatch pooledCounter;
	private double averageTicksPerAccess = 20;
	private long lastupdate = 0;
	//this is an example of how to implement PooledGameObjects in a class
	// Use this for initialization
	void Start () {
		testObj.SetActive(false); // to prevent the test object from destroying itself if that field has been set

		Stopwatch timer = new Stopwatch();
		pooledCounter = new Stopwatch();

		timer.Start();
		//NOTE: EventSystem is the object that contains the PGOManager as a component. If the parent class changes, rename EventSystem to whatever the name 
				//of the parent in the scene is. 

		pgoManager = GameObject.Find("EventSystem").GetComponent<PGOManager>();
		UnityEngine.Debug.Log(pgoManager);


		//this initializes the object. Use the same key to access a copy of the initialized object.
		pgoManager.InitObject(GetType(), testObj);


		timer.Stop();
		UnityEngine.Debug.Log("PGO Loadtime: " + timer.ElapsedMilliseconds + "ms");

		pooledCounter.Start();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButton("Fire1"))
        {
			Stopwatch t = new Stopwatch();
			t.Start();
			if(disablePGO){
				GameObject g = Instantiate(testObj, this.gameObject.transform);
				g.transform.position = this.gameObject.transform.position;
				g.SetActive(true);
			}else{
				//it's very important to remember that the state of this object could be anything.
				//It could be anywhere, with any variable set. The only guarentee is that the object is inactive. 
				GameObject g = pgoManager.GetPooledObject(this.GetType());

				//RESET ALL VARIABLES HERE (timers, position, states, etc...).
				g.transform.position = this.gameObject.transform.position;
				g.SetActive(true);
			}
			t.Stop();
			
			averageTicksPerAccess = 0.01 * t.ElapsedTicks + 0.99 * averageTicksPerAccess;
        }



		if(pooledCounter.ElapsedMilliseconds - lastupdate > 1000){
			lastupdate = pooledCounter.ElapsedMilliseconds;
			UnityEngine.Debug.Log(averageTicksPerAccess);
		}
	}
}

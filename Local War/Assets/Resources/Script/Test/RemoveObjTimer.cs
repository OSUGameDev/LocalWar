using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class RemoveObjTimer : MonoBehaviour {

	//set to true to have the object destroy when the timer ends
	public bool destroy = false;
	
	//how long the object should take to timeout in milliseconds
	public int timeout = 2000;

	private Stopwatch timer;
	// Use this for initialization
	
	void OnEnable(){
		if(timer == null){
			timer = new Stopwatch();
		}
		timer.Reset();
		timer.Start();
	}
	
	// Update is called once per frame
	void Update () {
		if(timer.ElapsedMilliseconds > timeout){
			if(destroy){
				Destroy(this.gameObject);
			}else{
				this.gameObject.SetActive(false);
			}
			timer.Stop();
		}
	}
}

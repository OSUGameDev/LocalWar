using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkFakeGame : MonoBehaviour {

	// Use this for initialization
	void Start () {
        if (NetworkManager.singleton != null)
        {
            Destroy(gameObject);
            return;
        }
        Instantiate(Resources.Load<GameObject>("Prefab/Networking/NetworkManager"));
        NetworkManager.singleton.StartHost();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

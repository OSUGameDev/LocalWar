using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputerController : MonoBehaviour {

    public GameObject testerPrefab;
    private GameObject tester;

    private void respawn()
    {
        tester = Instantiate(testerPrefab, transform.position, transform.rotation);
    }

    // Use this for initialization
    void Start () {
        testerPrefab = Resources.Load<GameObject>("Prefab/Character/Tester");
        respawn();
    }
	
	// Update is called once per frame
	void Update () {
        if (tester == null)
            respawn();
	}
}

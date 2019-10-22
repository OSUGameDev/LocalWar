using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public  GameObject playerPrefab;
    private GameObject player;

    private void respawn()
    {
        player = Instantiate(playerPrefab, transform.position, transform.rotation);
    }

    // Use this for initialization
    void Start()
    {
        playerPrefab = Resources.Load<GameObject>("Prefab/Character/Player");
        respawn();
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
            respawn();
    }
}

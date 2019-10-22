using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour {

    public  GameObject playerPrefab;
    private GameObject player;

    [Command]
    private void CmdRespawn()
    {
        player = Instantiate(playerPrefab, transform.position, transform.rotation);
        NetworkServer.SpawnWithClientAuthority(player, connectionToClient);
    }

    // Use this for initialization
    void Start()
    {
        playerPrefab = Resources.Load<GameObject>("Prefab/Character/Player");
        CmdRespawn();
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
            CmdRespawn();
    }
}

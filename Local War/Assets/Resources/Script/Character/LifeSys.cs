using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LifeSys : NetworkBehaviour, ISpawnable {

    public static LifeSys playerLifeSystem;
    public TimeSpan respawnTime = new TimeSpan(0, 0, 10);

    /// <summary>
    /// Health of the entity
    /// </summary>
    public float           health
    {
        get {
            return _health;
        }
        private set {
            _health = value;
            if (_health <= 0)
            {
                _health = 0;
                if(isServer)
                    Kill();
            }
        }
    }
    
    /// <summary>
    /// Backend field for the <see cref="Health"/> property
    /// </summary>
    [SyncVar]
    private float           _health;

    /// <summary>
    /// Shield of the entity
    /// </summary>
    public float           shield
    {
        get {
            return _shield;
        }
        private set {
            _shield = value;
            if (_shield <= 0)
                _shield = 0;
        }
    }

    /// <summary>
    /// Backend field for the <see cref="shield"/> property
    /// </summary>
    [SyncVar]
    private float _shield;

    public DateTime nextSpawnTime;

    //private List<Buff>      buffList;
    //private List<Debuff>    debuffList;


    [ClientRpc]
    private void RpcKill()
    {
        nextSpawnTime = DateTime.Now + respawnTime;
        gameObject.SetActive(false);
        if(hasAuthority)
        {
            GameObject.Find("RespawnCamera").GetComponent<Camera>().enabled = true;
        }
    }

    [ClientRpc]
    private void RpcActivate()
    {
        gameObject.SetActive(true);
    }

    [ClientRpc]
    private void RpcSpawn()
    {
        if (hasAuthority)
        {
            transform.position = GameObject.FindObjectsOfType<NetworkStartPosition>()[0].transform.position;
            transform.rotation = GameObject.FindObjectsOfType<NetworkStartPosition>()[0].transform.rotation;
            GameObject.Find("RespawnCamera").GetComponent<Camera>().enabled = false;
        }
        gameObject.SetActive(true);
    }

    [ClientRpc]
    private void RpcClientDamaged(float dmg)
    {
    }

    [ClientRpc]
    private void RpcSetRespawnTime(long binaryTime)
    {
    }

    [Server]
    public void Kill()
    {
        gameObject.SetActive(false);
        RpcKill();
        RespawnManager.singleton.QueueRespawn(respawnTime, this);
    }

    [Server]
    public void InflictDamage(float dmg)
    {
        RpcClientDamaged(dmg);
        if (shield > 0)
        {
            shield -= dmg;
        }
        else
        {
            health -= dmg;
        }
    }

    [Server]
    public void Spawn(int health, int shield)
    {
        _health = health;
        _shield = shield;
        RpcSpawn();
    }

    [Server]
    public void Spawn()
    {
        Spawn(100, 100);
    }

    void Start () {
        _health = 100.0f;
        _shield = 100.0f;
        if (hasAuthority)
            playerLifeSystem = this;
    }

	void Update () {
	}
}

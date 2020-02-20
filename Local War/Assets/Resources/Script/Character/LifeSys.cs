using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LifeSys : NetworkBehaviour, ISpawnable {

    
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

    private bool pIsAlive;
    public bool isAlive => pIsAlive;

    [ClientRpc]
    protected virtual void RpcKill()
    {
        pIsAlive = false;
        nextSpawnTime = DateTime.Now + respawnTime;
        gameObject.SetActive(false);
        if(hasAuthority)
        {
            GameObject.Find("RespawnCamera").GetComponent<Camera>().enabled = true;
            GameObject.Find("RespawnCamera").GetComponent<AudioListener>().enabled = true;
        }
    }

    [ClientRpc]
    private void RpcActivate()
    {
        gameObject.SetActive(true);
    }

    protected virtual Transform getSpawnLocation()
    {
        return GameObject.FindObjectsOfType<NetworkStartPosition>()[0].transform;
    }

    [ClientRpc]
    private void RpcSpawn()
    {
        pIsAlive = true;
        if (hasAuthority)
        {
            var spawnPos = getSpawnLocation();
            transform.position = spawnPos.position;
            transform.rotation = spawnPos.rotation;
            GameObject.Find("RespawnCamera").GetComponent<Camera>().enabled = false;
            GameObject.Find("RespawnCamera").GetComponent<AudioListener>().enabled = false;
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
    public virtual void Kill()
    {
        pIsAlive = false;
        gameObject.SetActive(false);
        RpcKill();
        RespawnManager.singleton.QueueRespawn(respawnTime, this);
    }

    [Server]
    public virtual void Kill(DateTime overrideRespawnTime)
    {
        pIsAlive = false;
        gameObject.SetActive(false);
        RpcKill();
        RespawnManager.singleton.QueueRespawn(overrideRespawnTime, this);
    }

    [Server]
    public virtual void InflictDamage(float dmg, int playerHashCode)
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
        pIsAlive = true;
        _health = health;
        _shield = shield;
        RpcSpawn();
    }

    [Server]
    public void Spawn()
    {
        Spawn(100, 100);
    }

    protected virtual void Start () {
        _health = 100.0f;
        _shield = 100.0f;
    }

	protected virtual void Update () {
	}
}

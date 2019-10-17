using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LifeSys : NetworkBehaviour {


    private uint DirtyBits;

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
                RpcDie();
            DirtyBits = DirtyBits | 0x00000001;
        }
    }
    /// <summary>
    /// Backend field for the <see cref="Health"/> property
    /// </summary>
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
            DirtyBits = DirtyBits | 0x00000002;
        }
    }

    /// <summary>
    /// Backend field for the <see cref="shield"/> property
    /// </summary>
    private float _shield;


    //private List<Buff>      buffList;
    //private List<Debuff>    debuffList;

    [ClientRpc]
    public void RpcDie()
    {
        gameObject.SetActive(false); 
    }

    [ClientRpc]
    public void RpcClientDamaged(float dmg)
    {
        Debug.Log("Receive damage! " + dmg);
    }


    [Server]
    public void InflictDamage(float dmg)
    {
        RpcClientDamaged(dmg);
        Debug.Log("Client "+netId+" recieved "+ dmg + " damage!");
        if (shield > 0)
        {
            shield -= dmg;
        }
        else
        {
            health -= dmg;
        }
    }

	// Use this for initialization
	void Start () {
        health = 100.0f;
        shield = 100.0f;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override System.Boolean OnSerialize(NetworkWriter writer, System.Boolean initialState)
    {
        if (!initialState && DirtyBits == 0)
            return false;
        writer.Write(health);
        writer.Write(shield);
        return true;
    }

    public override void OnDeserialize(NetworkReader reader, System.Boolean initialState)
    {
        _health = reader.ReadSingle();
        _shield = reader.ReadSingle();
    }

}

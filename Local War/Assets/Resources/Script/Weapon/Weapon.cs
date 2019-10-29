using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Weapon : NetworkBehaviour {
    protected int       id;
    protected int           type;
    protected string        description;
    protected Camera        playerCame;
    //protected WeaponSys     player;
    public    GameObject    customUI;
    protected GameObject    customUIInstance;

    public virtual void Fire(bool isServer)
    {

    }

    public void SetDescription(string des)
    {
        description = des;
    }

    public void SetCamera(Camera targetCamera)
    {
        playerCame = targetCamera;
    }

    public virtual GameObject CustomUI()
    {
        customUIInstance.SetActive(true);
        //Return the instance
        return customUIInstance;
    }

    void Start()
    {

    }
}

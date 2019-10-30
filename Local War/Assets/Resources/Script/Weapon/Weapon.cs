using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Weapon : NetworkBehaviour {
    protected int           id;
    protected int           type;
    protected string        description;
    protected Camera        playerCame;
    protected bool          isShooting;
    protected bool          isFinishShoot;
    public    GameObject    customUI;
    protected GameObject    customUIInstance;

    public virtual void Fire()
    {

    }

    public void SetDescription(string des)
    {
        description = des;
    }

    //The function to set current weapon's camera, used to Aimming, only runs on server
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

    //Checking function, only runs on server
    public bool IsShooting()
    {
        return isShooting;
    }

    void Start()
    {

    }
}

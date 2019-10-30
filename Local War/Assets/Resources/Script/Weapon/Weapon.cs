using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Weapon : NetworkBehaviour {
<<<<<<< HEAD
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
=======
    protected int       id;
    protected int       type;
    protected string    description;
    protected Camera    playerCame;
>>>>>>> parent of 1c1b9bf... Laser Rifle v0.2

    public void SetDescription(string des)
    {
        description = des;
    }

    //The function to set current weapon's camera, used to Aimming, only runs on server
    public void SetCamera(Camera targetCamera)
    {
        playerCame = targetCamera;
    }
<<<<<<< HEAD

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
=======
>>>>>>> parent of 1c1b9bf... Laser Rifle v0.2
}

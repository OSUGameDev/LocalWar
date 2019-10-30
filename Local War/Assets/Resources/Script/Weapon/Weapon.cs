using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

<<<<<<< HEAD
public class Weapon : NetworkBehaviour
{
    protected int id;
    protected int type;
    protected string description;
    protected Camera playerCame;
    protected bool isShooting;
    protected bool isFinishShoot;
    public GameObject customUI;
    protected GameObject customUIInstance;
=======
public class Weapon : NetworkBehaviour {
    protected int       id;
    protected int           type;
    protected string        description;
    protected Camera        playerCame;
    //protected WeaponSys     player;
    public    GameObject    customUI;
    protected GameObject    customUIInstance;
>>>>>>> parent of abd412a... Laser Rifle v0.3

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
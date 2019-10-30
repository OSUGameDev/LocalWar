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
<<<<<<< HEAD
<<<<<<< HEAD
=======
>>>>>>> parent of abd412a... Laser Rifle v0.3
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
=======
    protected int       type;
    protected string    description;
    protected Camera    playerCame;
>>>>>>> parent of 1c1b9bf... Laser Rifle v0.2

    public void SetDescription(string des)
    {
        description = des;
    }

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

    void Start()
    {

    }
}
=======
}
>>>>>>> parent of 1c1b9bf... Laser Rifle v0.2

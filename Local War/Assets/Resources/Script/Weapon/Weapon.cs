using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Weapon : MonoBehaviour {
    protected int           id;
    protected int           type;
    protected string        description;
    protected WeaponSys     playerWeaponSys;
    protected bool          isShooting;
    protected bool          isFinishShoot;
    public    GameObject    customUI;
    protected GameObject    customUIInstance;

    public virtual void Fire()
    {

    }

    public virtual void Shoot(Vector3 destination)
    {

    }

    public void SetDescription(string des)
    {
        description = des;
    }

    //The function to set current weapon's camera, used to Aimming, only runs on server
    public void SetPlayer(WeaponSys targetPlayer)
    {
        playerWeaponSys = targetPlayer;
    }

    public virtual GameObject CustomUI()
    {
        if(customUIInstance != null)
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

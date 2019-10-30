using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WeaponSys : NetworkBehaviour {

    private     int         currentWeaponPos;
    private     Camera      playerCam;
    private     GameObject  weaponList;
    private     GameObject  currentWeapon;

    public void AddWeapon()
    {

    }

    public void DropWeapon()
    {

    }

    public void SwitchWeapon()
    {

    }

    public void ShowWeapon()
    {

    }

    // Use this for initialization
    void Start ()
    {
        //Initialize the objects
        playerCam = transform.Find("Main Camera").GetComponent<Camera>();
        weaponList = transform.Find("Weapons").gameObject;

<<<<<<< HEAD
        /*****Initialize the weapon*****/

        //Select initial weapon
        currentWeapon = weaponList.transform.GetChild(1).GetComponent<Weapon>();
        currentWeaponPos = 1;

        //Set the custom UI, only the player's object will execute this
        if (isLocalPlayer)
        {
            GameObject cUI = currentWeapon.CustomUI();
            cUI.transform.SetParent(GameObject.Find("PlayerUI").transform);
        }
    }
=======
        //Initialize the weapon
        currentWeapon = weaponList.transform.GetChild(1).gameObject;
        currentWeaponPos = 1;
	}
>>>>>>> parent of 1c1b9bf... Laser Rifle v0.2
	
	// Update is called once per frame
	void Update ()
    {
<<<<<<< HEAD
        if(hasAuthority && isLocalPlayer)
=======
        if(hasAuthority)
        if (Input.GetButton("Fire1"))
>>>>>>> parent of 1c1b9bf... Laser Rifle v0.2
        {
            CmdFire();
        }
    }

    [Command]
    void CmdFire()
    {
        RpcFire();
    }

    [ClientRpc]
    //This will called on all instance of this player
    void RpcFire()
    {
        //Get the access to the target
        RangeWeapon script = currentWeapon.GetComponent<RangeWeapon>();

        //Set the camera then perform attack
<<<<<<< HEAD
        currentWeapon.SetCamera(playerCam);
        currentWeapon.Fire();
=======
        script.SetCamera(playerCam);
        script.Fire(isServer);
>>>>>>> parent of 1c1b9bf... Laser Rifle v0.2
    }

}

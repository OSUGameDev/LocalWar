using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WeaponSys : NetworkBehaviour {

    private     int         currentWeaponPos;
    private     Camera      playerCam;
    private     GameObject  weaponList;
    private     Weapon      currentWeapon;

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

    public void AddUI(GameObject customUI)
    {
        if(isLocalPlayer)
        {

        }
    }

    // Use this for initialization
    void Start ()
    {
        //Initialize the objects
        playerCam = transform.Find("Main Camera").GetComponent<Camera>();
        weaponList = transform.Find("Weapons").gameObject;

        /*****Initialize the weapon*****/

        //Select initial weapon
        currentWeapon = weaponList.transform.GetChild(1).GetComponent<Weapon>();
        //Set the custom UI
        if(isLocalPlayer)
        {
            GameObject cUI = currentWeapon.CustomUI();
            cUI.transform.SetParent(GameObject.Find("PlayerUI").transform);
        }
        currentWeaponPos = 1;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(hasAuthority)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                CmdFire();
            }
        }     
    }

    [Command]
    void CmdFire()
    {
        //All the game object in all client will be called with this function
        RpcFire();
    }

    [ClientRpc]
    void RpcFire()
    {
        //Set the camera then perform attack
        currentWeapon.SetCamera(playerCam);
        currentWeapon.Fire(isServer);
    }

}

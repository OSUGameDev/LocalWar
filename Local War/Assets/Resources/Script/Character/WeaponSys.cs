using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WeaponSys : NetworkBehaviour
{

<<<<<<< HEAD
    private int currentWeaponPos;
    private Camera playerCam;
    private GameObject weaponList;
    private Weapon currentWeapon;
=======
    private     int         currentWeaponPos;
    private     Camera      playerCam;
    private     GameObject  weaponList;
    private     GameObject  currentWeapon;
>>>>>>> parent of 1c1b9bf... Laser Rifle v0.2

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

<<<<<<< HEAD
    public void AddUI(GameObject customUI)
    {
        if (isLocalPlayer)
        {

        }
    }

=======
>>>>>>> parent of 1c1b9bf... Laser Rifle v0.2
    // Use this for initialization
    void Start()
    {
        //Initialize the objects
        playerCam = transform.Find("Main Camera").GetComponent<Camera>();
        weaponList = transform.Find("Weapons").gameObject;

<<<<<<< HEAD
        /*****Initialize the weapon*****/

        //Select initial weapon
        currentWeapon = weaponList.transform.GetChild(1).GetComponent<Weapon>();
        //Set the custom UI
        if(isLocalPlayer)
        {
            GameObject cUI = currentWeapon.CustomUI();
            cUI.transform.SetParent(GameObject.Find("PlayerUI").transform);
        }
<<<<<<< HEAD
    }

    // Update is called once per frame
    void Update()
    {
        if (hasAuthority && isLocalPlayer)
=======
=======
        //Initialize the weapon
        currentWeapon = weaponList.transform.GetChild(1).gameObject;
>>>>>>> parent of 1c1b9bf... Laser Rifle v0.2
        currentWeaponPos = 1;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(hasAuthority)
<<<<<<< HEAD
>>>>>>> parent of abd412a... Laser Rifle v0.3
        {
            if (Input.GetButtonDown("Fire1"))
            {
                CmdFire();
            }
=======
        if (Input.GetButton("Fire1"))
        {
            CmdFire();
>>>>>>> parent of 1c1b9bf... Laser Rifle v0.2
        }
    }

    [Command]
    void CmdFire()
    {
        RpcFire();
    }

    [ClientRpc]
    void RpcFire()
    {
        //Get the access to the target
        RangeWeapon script = currentWeapon.GetComponent<RangeWeapon>();

        //Set the camera then perform attack
        script.SetCamera(playerCam);
        script.Fire(isServer);
    }

}
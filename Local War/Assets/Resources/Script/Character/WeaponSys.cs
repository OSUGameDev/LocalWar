using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WeaponSys : NetworkBehaviour
{

    private int currentWeaponPos;
    private Camera playerCam;
    private GameObject weaponList;
    private Weapon currentWeapon;

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
        if (isLocalPlayer)
        {

        }
    }

    // Use this for initialization
    void Start()
    {
        //Initialize the objects
        playerCam = transform.Find("Main Camera").GetComponent<Camera>();
        weaponList = transform.Find("Weapons").gameObject;

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

    // Update is called once per frame
    void Update()
    {
        if (hasAuthority && isLocalPlayer)
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
        RpcFire();
    }

    [ClientRpc]
    //This will called on all instance of this player
    void RpcFire()
    {
        //Set the camera then perform attack
        currentWeapon.SetCamera(playerCam);
        currentWeapon.Fire();
    }

}
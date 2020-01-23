using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(CharacterLifeSystems))]
public class WeaponSys : NetworkBehaviour {

    private     int         currentWeaponPos;
    private     Camera      playerCam;
    private     GameObject  weaponList;
    private     GameObject  currentWeapon;
    private CharacterLifeSystems currentCharacter;

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
        currentCharacter = this.GetComponent<CharacterLifeSystems>();

        //Initialize the weapon
        currentWeapon = weaponList.transform.GetChild(0).gameObject;
        currentWeaponPos = 0;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(hasAuthority)
        if (Input.GetButton("Fire1"))
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
    void RpcFire()
    {
        //Get the access to the target
        RangeWeapon script = currentWeapon.GetComponent<RangeWeapon>();

        SoundSys.PlaySound(script.fireSound);

        //Set the camera then perform attack
        script.SetCamera(playerCam);
        script.Fire(isServer, currentCharacter.info.Team);
    }

}

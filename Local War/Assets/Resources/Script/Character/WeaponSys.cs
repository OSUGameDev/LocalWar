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
        currentWeapon.SetPlayer(this);
        currentWeapon.Fire();
    }

    //The function will be called by the range weapon to trigger fire effect
    [Command]
    public void CmdRayCastFirePlayer(GameObject muzzle, float damage, float weaponAccuracy)
    {
        Debug.Log("Triggered on server!");
        Ray ray = playerCam.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
        //Find the target point
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            //Check if the target is a player
            LifeSys target = hit.collider.gameObject.GetComponent<LifeSys>();
            //If the target is a player
            if(target != null)
            {
                //Make damage to the target player
                target.InflictDamage(damage);
            }

            //Draw the trail anyway

            /*****Modify the end point based on accuracy*****/
            RpcDrawbullet(hit.point);
        }
    }

    //Draw the trail on all client
    [ClientRpc]
    public void RpcDrawbullet(Vector3 destination)
    {
        Debug.Log("Triggered on client!");
        currentWeapon.Shoot(destination);
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
        currentWeaponPos = 1;

        //Set the custom UI, only the player's object will execute this
        if (isLocalPlayer)
        {
            GameObject cUI = currentWeapon.CustomUI();
            cUI.transform.SetParent(GameObject.Find("PlayerUI").transform);
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        if(hasAuthority && isLocalPlayer)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                CmdFire();
            }
        }     
    }

}

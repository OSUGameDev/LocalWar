using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LaserPistol : RangeWeapon {

    public override void Fire()
    {
        if (!isShooting)
        {
            isShooting = true;

            //Call the ray casting on the server
            playerWeaponSys.CmdRayCastFirePlayer(
                ammoType, 
                1.0f, 
                ammoType.GetComponent<Ammo>().returnDmg());
        }
    }

    // Use this for initialization
    void Start () {
        coolDown = 0.2f;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}

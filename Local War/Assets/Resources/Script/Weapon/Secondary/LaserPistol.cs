using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LaserPistol : RangeWeapon {

    public override void Fire()
    {
        Debug.Log("Pistol fire!");
        if (!isShooting)
        {
            isShooting = true;

            //Call the ray casting on the server
            playerWeaponSys.CmdRayCastFirePlayer(
                ammoType,
                ammoType.GetComponent<Ammo>().returnDmg(),
                1.0f);

            coolDownCounter = coolDown;
        }
    }

    public override void Shoot(Vector3 destination)
    {
        Debug.Log("Pistol shoot!");
        GameObject bullet = Instantiate(ammoType, firePoint.transform.position, firePoint.transform.rotation);
        Ammo script = bullet.GetComponent<Ammo>();
        script.setOrigin(firePoint.transform.position);
        script.initialize(destination);

        isFinishShoot = true;
    }

    // Use this for initialization
    void Start () {
        coolDown = 0.01f;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}

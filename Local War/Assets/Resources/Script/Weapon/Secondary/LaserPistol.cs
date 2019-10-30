using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LaserPistol : RangeWeapon {

    [Command]
    void CmdRayCast()
    {
        Ray ray = playerCame.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
        //Find the target point
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            RpcShootLaser(hit);
        }
    }

    [ClientRpc]
    void RpcShootLaser(RaycastHit hit)
    {
        GameObject bullet = Instantiate(ammoType, firePoint.transform.position, firePoint.transform.rotation);
        Ammo script = bullet.GetComponent<Ammo>();
        script.setOrigin(firePoint.transform.position);
        script.initialize(hit);
    }

    public override void Fire()
    {
        CmdRayCast();

        isShooting = true;
        coolDownCounter = coolDown;
    }

    // Use this for initialization
    void Start () {
        coolDown = 0.2f;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}

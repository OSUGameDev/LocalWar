using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserPistol : RangeWeapon {

    public override void Fire(bool isServer)
    {
        //Cast a ray from center of the camera
        Ray ray = playerCame.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
        //Find the target point
        RaycastHit hit;
        //If the ray hit something
        if (Physics.Raycast(ray, out hit) && !isShooting)
        {
            GameObject bullet = Instantiate(ammoType, firePoint.transform.position, firePoint.transform.rotation);
            Ammo script = bullet.GetComponent<Ammo>();
            script.setOrigin(firePoint.transform.position);
            script.initialize(hit, isServer);
            isShooting = true;
            coolDownCounter = coolDown;
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

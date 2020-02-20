﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeWeapon : Weapon{

    private int     ammo;
    private int     magzine;
    private int     totalAmmo;
    private float   coolDown;
    private float   coolDownCounter;
    private bool    isShooting;

    public GameObject ammoType;
    public GameObject firePoint;

    public void Fire(bool isServer, int playerHashCode)
    {
        //Cast a ray from center of the camera
        Ray ray = playerCame.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
        //Find the target point
        RaycastHit hit;
        //If the ray hit something
        if (Physics.Raycast(ray, out hit) && !isShooting)
        {
            GameObject bullet = Instantiate(ammoType, firePoint.transform.position, firePoint.transform.rotation);
            LaserAmmo script = bullet.GetComponent<LaserAmmo>();
            script.setOrigin(firePoint.transform.position);
            script.initialize(hit.point, isServer, playerHashCode);
            isShooting = true;
            coolDownCounter = coolDown;
        }
    }
    
	// Use this for initialization
	void Start () {
        coolDown = 0.2f;
        isShooting = false;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if(isShooting)
        {
            coolDownCounter -= Time.fixedDeltaTime;
            if (coolDownCounter <= 0)
            {
                isShooting = false;
            }
        }     
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeWeapon : Weapon{

    protected int     ammo;
    protected int     magzine;
    protected int     totalAmmo;
    protected float   coolDown;
    protected float   coolDownCounter;
    protected bool    isShooting;

    public GameObject ammoType;
    public GameObject firePoint;

    protected void CoolDown()
    {
        if (isShooting)
        {
            coolDownCounter -= Time.fixedDeltaTime;
            if (coolDownCounter <= 0)
            {
                isShooting = false;
            }
        }
    }
    
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void FixedUpdate () {
        CoolDown();     
	}
}

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeWeapon : Weapon
{

<<<<<<< HEAD
    protected int ammo;
    protected int magzine;
    protected int totalAmmo;
    protected float coolDown;
    protected float coolDownCounter;
=======
    protected int     ammo;
    protected int     magzine;
    protected int     totalAmmo;
    protected float   coolDown;
    protected float   coolDownCounter;
    protected bool    isShooting;
<<<<<<< HEAD
>>>>>>> parent of abd412a... Laser Rifle v0.3
=======
>>>>>>> parent of abd412a... Laser Rifle v0.3

    public GameObject ammoType;
    public GameObject firePoint;

<<<<<<< HEAD
    public virtual void Fire(bool isServer)
    {
        
    }

=======
>>>>>>> parent of abd412a... Laser Rifle v0.3
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
<<<<<<< HEAD
<<<<<<< HEAD

<<<<<<< HEAD
    // Update is called once per frame
    void FixedUpdate()
    {
        if (isFinishShoot)
            CoolDown();
    }
}
=======
=======
        isShooting = false;
>>>>>>> parent of 1c1b9bf... Laser Rifle v0.2
=======

>>>>>>> parent of abd412a... Laser Rifle v0.3
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        CoolDown();     
	}
}
>>>>>>> parent of abd412a... Laser Rifle v0.3

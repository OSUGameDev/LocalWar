using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RangeWeapon : Weapon{

    protected int     ammo;
    protected int     magzine;
    protected int     totalAmmo;
    protected float   coolDown;
    protected float   coolDownCounter;

    public GameObject ammoType;
    public GameObject firePoint;

<<<<<<< HEAD
    //The count down function, used after each shot
=======
    public virtual void Fire(bool isServer)
    {
        
    }

>>>>>>> parent of 1c1b9bf... Laser Rifle v0.2
    protected void CoolDown()
    {
        if (isShooting)
        {
            coolDownCounter -= Time.fixedDeltaTime;
            if (coolDownCounter <= 0)
            {
                isShooting = false;
                isFinishShoot = false;
            }
        }
    }
<<<<<<< HEAD

    // Update is called once per frame
    void FixedUpdate () {
        if(isFinishShoot)
            CoolDown();     
=======
    
	// Use this for initialization
	void Start () {
        isShooting = false;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        CoolDown();     
>>>>>>> parent of 1c1b9bf... Laser Rifle v0.2
	}
}

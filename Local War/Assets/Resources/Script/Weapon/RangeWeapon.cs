using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RangeWeapon : Weapon
{

    protected int ammo;
    protected int magzine;
    protected int totalAmmo;
    protected float coolDown;
    protected float coolDownCounter;

    public GameObject ammoType;
    public GameObject firePoint;

    //The count down function, used after each shot
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

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isFinishShoot)
            CoolDown();
    }
}
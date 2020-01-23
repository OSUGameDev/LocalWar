using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIWeaponSys : MonoBehaviour
{
    // Start is called before the first frame update

    // Goal: Create a program which can successfully attack another player
    // - Function to fire when player is in sight - called when enemy player sighted?
    // -- AI fires towards a "box" around the player, tries to fire towards specific areas (torso, head)
    // --- might hopefully give player a chance

    //Movement systems and AI movement systems similar
    // - adapt WeaponSys to AI WeaponSys

    private int currentWeaponPos;
  
    private GameObject weaponList;
    private GameObject currentWeapon;
    //Variables from Range Weapon - This is probably very inefficient
    private bool isShooting;
    private float coolDown;
    private float coolDownCounter;
    public GameObject ammoType;
    
    void Start()
    {
        isShooting = false;
        coolDown = 1;

        //Initializes the object
        weaponList = transform.Find("Weapons").gameObject;

        //Initalize the weapon
        currentWeapon = weaponList.transform.GetChild(0).gameObject;
        currentWeaponPos = 0;

    }

    // Update is called once per frame
    void Update()
    {
        //Check if the AI is shooting
        if(isShooting)
        {
            //If so, reduces cooldown, and sets shooting to false if cooldown is zero or negative.
            coolDownCounter -= Time.fixedDeltaTime;
            if (coolDownCounter <= 0)
            {
                isShooting = false;
            }
        }
        else {
            //If the AI is not shooting, lets the AI shoot
            //CHANGE CONDITION TO SOMETHING WHICH CHECKS IF THE AI SEES A PLAYER
            if (true) {
                FireRange();
            }

        }
    }

    void FireRange() {

        transform.Rotate(0, 10, 0);
        //Vector3.RotateTowards(transform.forward, Camera.main.transform.position, 100 * Time.deltaTime, 0.0f);

        //Ray cast and find the fire point
        RaycastHit hit;
        bool ray = Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit);

        //If the ray hit something, fires a bullet and sets values to ensure that the player cannot constantly fire.
        if (ray)
        {
            GameObject bullet = Instantiate(ammoType, transform.position, transform.rotation);
            LaserAmmo laser = bullet.GetComponent<LaserAmmo>();
            laser.setOrigin(transform.position);
            laser.initialize(hit.point, true);
            isShooting = true;
            coolDownCounter = coolDown;
        }

    }
}

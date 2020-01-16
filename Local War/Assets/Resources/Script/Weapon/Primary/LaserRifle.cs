using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserRifle : RangeWeapon {

    private float accuracy;
    private bool isCharging;
    private Vector2[] uiPos;

    //Used for initialization
    private void StoreUIPos()
    {
        //Initialize the UI store list
        uiPos = new Vector2[4];
        //Store the initial position
        for (int i = 0; i < 4; i++)
        {
            RectTransform current = customUIInstance.transform.GetChild(i).GetComponent<RectTransform>();
            float x = current.anchoredPosition.x;
            float y = current.anchoredPosition.y;
            uiPos[i] = new Vector2(x, y);
        }
    }

    //One of the charging effect, runs on client
    private void ShrinkUI()
    {
        for (int i = 0; i < 4; i++)
        {
            RectTransform current = customUIInstance.transform.GetChild(i).GetComponent<RectTransform>();
            float x = current.anchoredPosition.x * 0.995f;
            float y = current.anchoredPosition.y * 0.995f;
            current.anchoredPosition = new Vector2(x, y);
        }
    }

    //This function used to reposition the UI after fire
    private void RepositionUI()
    {
        for (int i = 0; i < 4; i++)
        {
            RectTransform current = customUIInstance.transform.GetChild(i).GetComponent<RectTransform>();
            current.anchoredPosition = uiPos[i];
        }
    }

    //This function will be called by the weaponSys script owned by the player, not the copy of other client
    public override GameObject CustomUI()
    {
        //Create UI instance
        customUIInstance = Instantiate(customUI);
        customUIInstance.SetActive(false);
        StoreUIPos();
        return base.CustomUI();
    }

    //This function will be called by the weaponSys script when player pull the trigger
    public override void Fire()
    {
        if (!isAttacking)
        {
            isAttacking = true;
            isCharging = true;
        }
    }

    public override void Attack(Vector3 destination)
    {
        GameObject bullet = Instantiate(ammoType, firePoint.transform.position, firePoint.transform.rotation);
        Ammo script = bullet.GetComponent<Ammo>();
        script.setOrigin(firePoint.transform.position);
        script.initialize(destination);
    }

    // Use this for initialization
    void Start ()
    {
        //Initialize the accuracy
        accuracy = 0.4f;
        coolDown = 2.0f;
        isCharging = false;
        finishedAttacking = false;
    }
	
	// Update is called once per frame
	void Update () {

        //Debug.Log(Input.GetMouseButton(0));
        if (isCharging && Input.GetButton("Fire1"))
        {
            //Check if the maximum charge
            if (accuracy < 1.0)
            {
                //Increase accuracy
                accuracy += 0.002f;
                //Shrink the crosshair
                ShrinkUI();
            }
        }
        else if(isCharging && Input.GetButtonUp("Fire1"))
        {
            isCharging = false;

            //Generate the prefab
            //ammoType = Instantiate(ammoType);

            Debug.Log(ammoType.GetComponent<Ammo>().returnDmg());

            //Call the ray casting on the server
            playerWeaponSys.CmdRayCastFirePlayer(
                ammoType,
                ammoType.GetComponent<Ammo>().returnDmg(), 
                accuracy);
            
            //Reset the parameter on all client
            accuracy = 0.4f;
            coolDownCounter = coolDown;
            RepositionUI();
            finishedAttacking = true;
        }
    }
}

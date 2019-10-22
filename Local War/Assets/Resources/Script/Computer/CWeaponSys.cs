using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CWeaponSys : MonoBehaviour {

    private     int         currentWeaponPos;
    private     GameObject  weaponList;
    private     GameObject  currentWeapon;

    public void AddWeapon()
    {

    }

    public void DropWeapon()
    {

    }

    public void SwitchWeapon()
    {

    }

    public void ShowWeapon()
    {

    }

    // Use this for initialization
    void Start ()
    {
        //Initialize the objects
        weaponList = transform.Find("Weapons").gameObject;

        //Initialize the weapon
        currentWeapon = weaponList.transform.GetChild(0).gameObject;
        currentWeaponPos = 0;
	}
	
	// Update is called once per frame
	void Update ()
    {

    }
}

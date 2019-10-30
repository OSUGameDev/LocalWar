using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LaserRifle : RangeWeapon
{

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
            float x = current.anchoredPosition.x;
            float y = current.anchoredPosition.y;
            current.anchoredPosition = uiPos[i];
        }
    }

    //This function will be called by the weaponSys script when player pull the trigger
    public override void Fire()
    {
        //If the ray hit something
        if (!isShooting)
        {
            isShooting = true;
            isCharging = true;
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

    // Use this for initialization
    void Start()
    {
        //Initialize the accuracy
        accuracy = 0.4f;
        coolDown = 2.0f;
        isCharging = false;
        isFinishShoot = false;
    }

    // Update is called once per frame
    void Update()
    {

        //Debug.Log(Input.GetMouseButton(0));
        if (isCharging && Input.GetButton("Fire1"))
        {
            //Debug.Log("Charging!");
            //Debug.Log(isCharging);
            //Check if the maximum charge
            if (accuracy < 1.0)
            {
                //Increase accuracy
                accuracy += 0.002f;
                //Shrink the crosshair
                ShrinkUI();
            }
        }
        else if (isCharging && Input.GetButtonUp("Fire1"))
        {
            isCharging = false;
            //Call the ray casting on the server
            CmdRayCast();

            //Reset the parameter on all client
            accuracy = 0.4f;
            coolDownCounter = coolDown;
            RepositionUI();
            isFinishShoot = true;
        }
    }
}
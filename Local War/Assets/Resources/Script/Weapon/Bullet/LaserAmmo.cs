using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserAmmo : Ammo
{

    public float maxRadius;

    private float angle;
    private Vector3 destination;
    private LineRenderer line;

    public void initialize(Vector3 direction, bool isServer, int playerHashCode)
    {
        damage = 15.0f;

        //If put the get function in Start then it will not execute properly
        line = GetComponent<LineRenderer>();

        destination = direction;
        line.SetPosition(0, origin);
        line.SetPosition(1, destination);

        if (!isServer)
            return;

        RaycastHit hit;
        if (Physics.Raycast(origin, destination - origin, out hit))
        {
            LifeSys target = hit.collider.gameObject.GetComponent<LifeSys>();
            if (target != null)
            {
                Debug.Log(maxRadius);
                target.InflictDamage(damage, playerHashCode);
            }
        }
    }

    void Start()
    {
        line.startWidth = 0;
        line.endWidth = 0;
        angle = 0;
        maxRadius = 0.01f;
    }

    void FixedUpdate()
    {
        line.startWidth = Mathf.Sin(angle) * maxRadius;
        line.endWidth = Mathf.Sin(angle) * maxRadius;

        angle += Mathf.PI * 10 / 180;
        if (angle >= Mathf.PI)
            Destroy(gameObject);
    }

}

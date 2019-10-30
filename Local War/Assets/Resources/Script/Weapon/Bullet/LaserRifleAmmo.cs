using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserRifleAmmo : Ammo {

    public float maxRadius;

    private float angle;
    private Vector3 destination;
    private LineRenderer line;

    public override void initialize(RaycastHit hit)
    {
        damage = 45.0f;

        //If put the get function in Start then it will not execute properly
        line = GetComponent<LineRenderer>();

        //Draw the laser on all client
        destination = hit.point;
        line.SetPosition(0, origin);
        line.SetPosition(1, destination);

        LifeSys target = hit.collider.gameObject.GetComponent<LifeSys>();
        if (target != null)
        {
            target.InflictDamage(damage);
        }
    }

    void Start()
    {
        line.startWidth = 0;
        line.endWidth = 0;
        angle = 0;
        maxRadius = 0.05f;
    }

    void FixedUpdate()
    {
        line.startWidth = Mathf.Sin(angle) * maxRadius;
        line.endWidth = Mathf.Sin(angle) * maxRadius;

        angle += Mathf.PI * 15 / 180;
        if (angle >= Mathf.PI)
            Destroy(gameObject);
    }
}

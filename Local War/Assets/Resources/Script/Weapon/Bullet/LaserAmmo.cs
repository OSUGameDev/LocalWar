using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserAmmo : Ammo {

    public      float           maxRadius;

    private     float           angle;
    private     Vector3         destination;
    private     LineRenderer    line;

<<<<<<< HEAD
    public override void initialize(RaycastHit hit)
=======
    public override void initialize(RaycastHit hit, bool isServer)
>>>>>>> parent of abd412a... Laser Rifle v0.3
    {
        damage = 15.0f;

        //If put the get function in Start then it will not execute properly
        line = GetComponent<LineRenderer>();
        line.SetPosition(0, origin);
        line.SetPosition(1, hit.point);

<<<<<<< HEAD
=======
        if (!isServer)
            return;

>>>>>>> parent of abd412a... Laser Rifle v0.3
        LifeSys target = hit.collider.gameObject.GetComponent<LifeSys>();
        if (target != null)
        {
            target.InflictDamage(damage);
        }
    }

    void Start ()
    {
        line.startWidth = 0;
        line.endWidth = 0;
        angle = 0;
        maxRadius = 0.01f;
    }

    void FixedUpdate ()
    {
        line.startWidth = Mathf.Sin(angle) * maxRadius;
        line.endWidth = Mathf.Sin(angle) * maxRadius;

        angle += Mathf.PI * 10 / 180;
        if (angle >= Mathf.PI)
            Destroy(gameObject);
    }

}

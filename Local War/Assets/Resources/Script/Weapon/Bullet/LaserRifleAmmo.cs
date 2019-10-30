using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserRifleAmmo : LaserAmmo {

    public float maxRadius;

    private float angle;
    private Vector3 destination;
    private LineRenderer line;

<<<<<<< HEAD
<<<<<<< HEAD
    public override void initialize(RaycastHit hit)
=======
    public override void initialize(RaycastHit hit, bool isServer)
>>>>>>> parent of abd412a... Laser Rifle v0.3
=======
    public override void initialize(RaycastHit hit, bool isServer)
>>>>>>> parent of abd412a... Laser Rifle v0.3
    {
        damage = 45.0f;

        //If put the get function in Start then it will not execute properly
        line = GetComponent<LineRenderer>();

        destination = hit.point;
        line.SetPosition(0, origin);
        line.SetPosition(1, destination);

<<<<<<< HEAD
<<<<<<< HEAD
=======
=======
>>>>>>> parent of abd412a... Laser Rifle v0.3
        if (!isServer)
            return;


<<<<<<< HEAD
>>>>>>> parent of abd412a... Laser Rifle v0.3
=======
>>>>>>> parent of abd412a... Laser Rifle v0.3
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

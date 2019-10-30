using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LaserAmmo : Ammo {

    public      float           maxRadius;

    private     float           angle;
    private     Vector3         destination;
    private     LineRenderer    line;

    public override void initialize(Vector3 destination)
    {
        damage = 15.0f;

        //If put the get function in Start then it will not execute properly
        line = GetComponent<LineRenderer>();

        //Draw the laser on all client
        line.SetPosition(0, origin);
        line.SetPosition(1, destination);
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

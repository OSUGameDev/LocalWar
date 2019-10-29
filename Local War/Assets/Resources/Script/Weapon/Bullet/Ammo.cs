using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;

public abstract class Ammo : NetworkBehaviour {

    protected   float       damage;
    protected   Vector3     origin;

    public virtual void initialize(Vector3 destination)
    {

    }

    public virtual float returnDmg()
    {
        return damage;
    }

    public void setOrigin(Vector3 originPoint)
    {
        origin = originPoint;
        transform.position = origin;
    }
}

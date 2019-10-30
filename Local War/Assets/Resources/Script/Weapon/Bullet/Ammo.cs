using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public abstract class Ammo : MonoBehaviour {

    protected   float       damage;
    protected   Vector3     origin;

    public virtual void initialize(RaycastHit hit, bool isServer)
    {

    }

    public void setOrigin(Vector3 originPoint)
    {
        origin = originPoint;
        transform.position = origin;
    }
}

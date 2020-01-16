using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public abstract class Ammo : MonoBehaviour {

    public   float       damage;
    protected   Vector3     origin;

    public void setOrigin(Vector3 originPoint)
    {
        origin = originPoint;
        transform.position = origin;
    }
}

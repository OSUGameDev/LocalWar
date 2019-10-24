using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {
    protected int       id;
    protected int       type;
    protected string    description;
    protected Camera    playerCame;

    public void SetDescription(string des)
    {
        description = des;
    }

    public void SetCamera(Camera targetCamera)
    {
        playerCame = targetCamera;
    }
}

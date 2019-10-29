using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Weapon : NetworkBehaviour {
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

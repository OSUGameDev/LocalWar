﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff : MonoBehaviour {

    protected string name;
    struct info
    {
        int paraNum;
        int targetType;
    }

    public void setName(string newName)
    {
        name = newName;
    }
}

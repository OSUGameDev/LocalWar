using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegenHealth : Buff {

    private float amountPer;

    public void effect(ref float health, float limit)
    {
        health += amountPer;
        if (health > limit)
            health = limit;
    }

	// Use this for initialization
	void Start () {
        setName("RegenHealth");
        amountPer = 10.0f;
    }
}

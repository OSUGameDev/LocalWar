using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeSys : MonoBehaviour {

    private float           health;
    private float           shield;
    private List<Buff>      buffList;
    private List<Debuff>    debuffList;

    public float ShowHealth()
    {
        return health;
    }

    public float ShowShield()
    {
        return shield;
    }

    public void Die()
    {
        gameObject.SetActive(false); 
    }

    public void ReceiveDamage(float dmg)
    {
        Debug.Log("Receive damage! " + dmg);
        if(shield > 0)
        {
            shield -= dmg;
            if (shield <= 0)
                shield = 0;
        }
        else
        {
            health -= dmg;
            if (health <= 0)
                Die();
        }
    }

	// Use this for initialization
	void Start () {
        health = 100.0f;
        shield = 100.0f;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class handlePit : MonoBehaviour
{
    private BoxCollider other;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //When the plane has been hit.
    //This would mean the player should die 
    //and respawn somewhere.
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("You have hit the pit");
    }
}

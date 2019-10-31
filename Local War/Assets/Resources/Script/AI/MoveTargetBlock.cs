using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTargetBlock : MonoBehaviour
{
    public LayerMask hitLayers;
    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0)){
            Vector3 mouse = Input.mousePosition;
            Ray castPoint = Camera.main.ScreenPointToRay(mouse); //cast a ray at the mouse pos
            RaycastHit hit; //store mousepos
            if(Physics.Raycast(castPoint, out hit, Mathf.Infinity, hitLayers)) //check if hitting wall
                this.transform.position = hit.point;
        }
    }
}

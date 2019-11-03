using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMovementSys : MonoBehaviour{

    private float speed = 2.0F;          //the moving speed of the character
    private float jumpSpeed = 10.0f;      //the jump force of the character
    private float rotateSpeed = 100.0f;

    private float gravity = 20.0f;       //the force of gravity on the character

    private float groundOffset = .2f;    //the offset for the IsGrounded check. Useful for recognizing slopes and imperfect ground.

    private float moveH, moveV;
    private float mouseX, mouseY;
    private Vector3 moveDirection = Vector3.zero;   //the direction the character should move.
    private Vector3 jumpDirection = Vector3.zero;
    private Quaternion rotationStart, rotationEnd;
    public Vector3 nextDestination;

    private Rigidbody rb;


    ///The check to see if the character is currently on the ground.
    private bool isGrounded()
    {
        RaycastHit hit;
        Physics.Raycast(this.transform.position, -this.transform.up, out hit, 10);   //A short ray shot directly downward from the center of the character.

        if (System.Math.Abs(hit.distance) < System.Single.Epsilon)                                           //if the distance is zero, the ray probably did not hit anything.
        {
            return false;
        }
        if (hit.distance <= (this.transform.lossyScale.y / 2 + groundOffset))   //if the distance from the ray is less than half the height 
        {                                                                   //of the character (plus the offset), the character us grounded.
            return true;
        }
        return false;
    }

    private void move()
    {
        Vector3 targetRot = nextDestination - transform.position;
        targetRot.y = 0.5f;
        //Vector3 newDir = Vector3.RotateTowards(transform.forward,targetRot,rotateSpeed*Time.deltaTime,0.0f);
        //Debug.DrawRay(transform.position, newDir, Color.red);
        //Lock rotation axis
        //transform.rotation = Quaternion.LookRotation(newDir);
        
        moveDirection = new Vector3(nextDestination.x, 0.5f,nextDestination.z);       //Create the player's movement from keyboard in local space
        //moveDirection = transform.TransformDirection(moveDirection);      //Transform the moveMent from local space to world space
        //moveDirection *= speed;      //Based on base speed

        /*****Check jump mode at last*****/
        /*if (Input.GetButtonDown("Jump"))               //jump if the character is grounded and the user presses the jump button.
        {
            jumpDirection.y = jumpSpeed;     //Give a jump speed to player
        }
        */

        /*****Move the character */
        transform.position = Vector3.MoveTowards(transform.position,moveDirection,speed*Time.deltaTime);    //move the character based on the gravitational force.
      
    }

    //This built-in function will be called after the script first time loaded into the scene
    void Start()
    {
       rb = gameObject.GetComponent<Rigidbody>();
        
    }

    void Update()
    {
        nextDestination = GetComponent<AstarPathfinding>().nextDestination; // get current node in astar graph
        move();
    }
}
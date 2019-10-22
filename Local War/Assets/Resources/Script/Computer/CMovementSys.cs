using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CMovementSys : MonoBehaviour {

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

    private CharacterController controller;


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
        /*****Move the player*****/
        controller.Move(moveDirection * Time.deltaTime);    //move the character based on the gravitational force.
        if (!controller.isGrounded)
            jumpDirection.y -= gravity * Time.deltaTime;
        else
            jumpDirection.y = 0;
        controller.Move(jumpDirection * Time.deltaTime);
    }

    //This built-in function will be called after the script first time loaded into the scene
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        move();
    }
}

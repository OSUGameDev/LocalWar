using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MovementSys : NetworkBehaviour {

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
        /*****Get basic player input*****/
        moveH = Input.GetAxis("Horizontal");
        moveV = Input.GetAxis("Vertical");
        mouseX += Input.GetAxis("Mouse X") * rotateSpeed * Time.deltaTime;
        mouseY -= Input.GetAxis("Mouse Y") * rotateSpeed * Time.deltaTime;
        mouseY = Mathf.Clamp(mouseY, -90, 90);

        rotationStart = transform.rotation;
        rotationEnd = Quaternion.Euler(mouseY, mouseX, 0f);
        transform.rotation = Quaternion.Lerp(rotationStart, rotationEnd, 0.5f);

        moveDirection = new Vector3(moveH, 0.0f, moveV);       //Create the player's movement from keyboard in local space
        moveDirection = transform.TransformDirection(moveDirection);      //Transform the moveMent from local space to world space
        moveDirection *= speed;      //Based on base speed

        /*****Check jump mode at last*****/
        if (Input.GetButtonDown("Jump"))               //jump if the character is grounded and the user presses the jump button.
        {
            jumpDirection.y = jumpSpeed;     //Give a jump speed to player
        }


        /*****Move the player*****/
        controller.Move(moveDirection * Time.deltaTime);    //move the character based on the gravitational force.
        if (!controller.isGrounded)
        {
            jumpDirection.y -= gravity * Time.deltaTime;
        }
        controller.Move(jumpDirection * Time.deltaTime);
    }

    //This built-in function will be called after the script first time loaded into the scene
    void Start()
    {
        mouseX = 0;
        mouseY = 0;

        controller = GetComponent<CharacterController>();
        transform.Find("Main Camera").GetComponent<Camera>().enabled = hasAuthority;
        if (hasAuthority)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void Update()
    {
        if (!hasAuthority)
            return;
        move();
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            if (isServer)
                NetworkManager.singleton.StopServer();
            NetworkManager.singleton.StopClient();
        }
    }
}

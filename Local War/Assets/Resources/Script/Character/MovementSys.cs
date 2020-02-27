using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MovementSys : NetworkBehaviour
{
    private Vector3 player_velocity;
    private Vector3 player_acceleration;
    private Vector3 previous_input_direction;
    private Vector3 input_direction;
    private Vector2 previous_mouse_axis;
    private Vector2 mouse_axis;

    private Vector3 force = new Vector3(18.0f, 0.0f, 18.0f);
    private float kinetic_friction = 0.12f;
    private float air_friction = 0.01f;
    private int jumps_used = 0;
    private int jump_count = 2;
   
    public float mass = 50.0f;
    public float gravity = 0.7f;
    public float jump_speed = 0.3f;

    private float sensitivity = 100.0f;
    private float player_height; //set on init.

    private CharacterController kinematic_controller;

    void Start()
    {
        player_velocity     = new Vector3();
        player_acceleration = new Vector3();
        input_direction     = new Vector2();
        mouse_axis          = new Vector2();
        player_height       = this.gameObject.GetComponent<Renderer>().bounds.size.y; //need this to calculate bottom bounds of the player's model. 

        kinematic_controller = GetComponent<CharacterController>();
        transform.Find("Main Camera").GetComponent<Camera>().enabled = hasAuthority;
        if (hasAuthority)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void GetInput()
    {
        // states
        previous_input_direction = input_direction;
        previous_mouse_axis = mouse_axis;

        // looking 
        // TODO: break modifying mouse movement from getting input
        mouse_axis.x += Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        mouse_axis.y -= Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
        mouse_axis.y = Mathf.Clamp(mouse_axis.y, -89.999f, 89.999f);
        Quaternion rotation_start = transform.rotation;
        Quaternion rotation_end = Quaternion.Euler(mouse_axis.y, mouse_axis.x, 0.0f);
        transform.rotation = Quaternion.Lerp(rotation_start, rotation_end, 0.5f);

        // movement
        input_direction.x = Input.GetAxis("Horizontal");
        input_direction.y = 0.0f;
        input_direction.z = Input.GetAxis("Vertical");

        // jumping:
        if (jumps_used < jump_count && Input.GetButtonDown("Jump"))
        {
            jumps_used++;
            // zero out jump velocity if falling
            if (player_velocity.y < 0.0f)
            {
                player_velocity.y = 0.0f;
            }
            player_velocity.y += jump_speed;
        }
    }

    private void Move()
    {
        // movement:
        // acceleration
        Vector3 acceleration = force / mass;

        // strafing
        if ( input_direction.x != 0.0f )
        {
            // get vector math
            Vector3 movement_forward = new Vector3(transform.forward.x, 0.0f, transform.forward.z); movement_forward.Normalize();
            Vector3 movement_cross = Vector3.Cross(movement_forward, Vector3.up);
            // apply input direction
            player_velocity = player_velocity - ( movement_cross * ( input_direction.x * acceleration.x * Time.deltaTime ) );
        }

        // forward/backwards
        if (input_direction.z != 0.0f)
        {
            // get vector math
            Vector3 movement_forward = new Vector3(transform.forward.x, 0.0f, transform.forward.z); movement_forward.Normalize();
            // apply input direction
            player_velocity = player_velocity + ( movement_forward * ( input_direction.z * acceleration.z * Time.deltaTime ) );
        }

        // friction
        Vector3 friction = new Vector3(-player_velocity.x * kinetic_friction, 
                                       -player_velocity.y * air_friction, 
                                       -player_velocity.z * kinetic_friction);
        player_velocity += friction;

        // gravity
        player_velocity.y -= gravity * Time.deltaTime;

        // apply movement
        kinematic_controller.Move(player_velocity);
    }

    void Update()
    {
        if (!hasAuthority)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            NetworkManager.singleton.StopClient();
        }

        GetInput();
    }

    private void FixedUpdate()
    {
        if (!hasAuthority)
            return;

        Move();

        if (kinematic_controller.isGrounded) //needs to run in fixed update, our isGrounded is unreliable.
        {
            jumps_used = 0;
            player_velocity.y = 0;
        }
    }

    private int signf( float val )
    {
        if (val == 0) return 0;
        else if (val > 0) return 1;
        else return -1;
    }
}

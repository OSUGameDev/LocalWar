using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MovementSys : NetworkBehaviour
{
    private Vector3 player_velocity;
    private Vector3 player_acceleration;
    public Vector3  acceleration;
    public float    kinetic_friction;
    public float    air_friction;
    private Vector3 previous_input_direction;
    private Vector3 input_direction;
    private Vector2 previous_mouse_axis;
    private Vector2 mouse_axis;
    public float sensitivity = 100.0f;

    private CharacterController kinematic_controller;

    private int jumps_used = 0;
    public int jump_count = 2;
    float gravity = 0.7f;
    float jump_speed = 0.3f;

    void Start()
    {
        acceleration        = new Vector3(1.8f, 0.0f, 1.8f);
        kinetic_friction    = 0.35f;
        air_friction        = 0.01f;

        player_velocity     = new Vector3();
        player_acceleration = new Vector3();
        input_direction     = new Vector2();
        mouse_axis          = new Vector2();

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
    }

    private void Move()
    {
		if (kinematic_controller.isGrounded) {
			jumps_used = 0;
			player_velocity.y = 0;
		}

        // jumping:
		if (jumps_used < jump_count && Input.GetButtonDown("Jump"))
        {
			jumps_used++;
            // zero out jump velocity if falling
            if(player_velocity.y < 0.0f)
            {
                player_velocity.y = 0.0f;
            }
            player_velocity.y += jump_speed;
        }

        // movement:

        // strafing
        if ( input_direction.x != 0.0f )
        {
            Vector3 movement_forward = new Vector3(transform.forward.x, 0.0f, transform.forward.z); movement_forward.Normalize();
            Vector3 movement_cross = Vector3.Cross(movement_forward, Vector3.up);
            player_velocity -= movement_cross * acceleration.x * input_direction.x * Time.deltaTime;
        }

        // forward/backwards
        if (input_direction.z != 0.0f)
        {
            Vector3 movement_forward = new Vector3(transform.forward.x, 0.0f, transform.forward.z); movement_forward.Normalize();
            player_velocity += movement_forward * acceleration.z * input_direction.z * Time.deltaTime;
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
    }

    private void FixedUpdate()
    {
        GetInput();
        Move();
    }

    private int signf( float val )
    {
        if (val == 0) return 0;
        else if (val > 0) return 1;
        else return -1;
    }
}

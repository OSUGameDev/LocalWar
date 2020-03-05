using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MovementSys : NetworkBehaviour
{
    private Vector3 player_velocity;
    private Vector3 player_acceleration;
    private Vector3 max_player_velocity;
    private Vector3 constant_acceleration;
    private Vector3 previous_input_direction;
    private Vector3 input_direction;
    private Vector2 previous_mouse_axis;
    private Vector2 mouse_axis;

    private Vector3 force = new Vector3(120.0f, 0.0f, 120.0f);
    private float kinetic_friction_constant = 0.12f;
    private float air_friction = 0.001f;
    private int jumps_used = 0;
    private int jump_count = 2;
    float mass = 50.0f;
    float gravity = 0.7f;
    float jump_speed = 0.03f;
    bool grounded = false;
    bool jump_flag = false;

    private float sensitivity = 100.0f;

    private CharacterController kinematic_controller;

    void Start()
    {
        player_velocity     = new Vector3();
        player_acceleration = new Vector3();
        constant_acceleration = new Vector3();
        input_direction     = new Vector2();
        mouse_axis          = new Vector2();
        set_max_player_velocity(new Vector3(0.05f, 0.0f, 0.05f));

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

        //sprinting
        if (Input.GetKey(KeyCode.LeftShift))
        {
            set_max_player_velocity(new Vector3(0.3f, 0.0f, 0.3f));
        }
        else
        {
            set_max_player_velocity(new Vector3(0.05f, 0.0f, 0.05f));
        }

        // jumping:
        if (jumps_used < jump_count && Input.GetButtonDown("Jump"))
        {
            jump_flag = true;
        }
    }

    private void Move()
    {
        // grounding:
        if (kinematic_controller.isGrounded)
        {
            grounded = true;
            jumps_used = 0;
            player_velocity.y = 0.0f;
        }
        else
        {
            grounded = false;
        }

        // jumping:
        if(jumps_used < jump_count && jump_flag)
        {
            // set meta
            jump_flag = false;
            jumps_used++;
            player_velocity.y = 0.0f;
            apply_impulse(new Vector3(0.0f, jump_speed, 0.0f), 60.0f);
        }

        // movement:

        // strafing
        if ( input_direction.x != 0.0f )
        {
            // get vector math
            Vector3 movement_forward = new Vector3(transform.forward.x, 0.0f, transform.forward.z); movement_forward.Normalize();
            Vector3 movement_cross = Vector3.Cross(movement_forward, Vector3.up);
            // apply input direction
            player_velocity = player_velocity - ( movement_cross * ( input_direction.x * constant_acceleration.x * Time.deltaTime ) );
        }

        // forward/backwards
        if (input_direction.z != 0.0f)
        {
            // get vector math
            Vector3 movement_forward = new Vector3(transform.forward.x, 0.0f, transform.forward.z); movement_forward.Normalize();
            // apply input direction
            player_velocity = player_velocity + ( movement_forward * ( input_direction.z * constant_acceleration.z * Time.deltaTime ) );
        }

        // friction
        Vector3 friction_velocity = new Vector3(-player_velocity.x * kinetic_friction_constant,
                                                -player_velocity.y * air_friction, 
                                                -player_velocity.z * kinetic_friction_constant);
        player_velocity += friction_velocity;

        // gravity
        player_velocity.y -= gravity * Time.deltaTime;

        // apply movement
        kinematic_controller.Move(player_velocity);
    }

    public void set_max_player_velocity( Vector3 new_vel )
    {
        // update max velocity
        max_player_velocity = new_vel;
        // update force/acceleration
        force = max_player_velocity * mass / (Time.deltaTime * (1.0f / kinetic_friction_constant - 1.0f));
        constant_acceleration = force / mass;
    }

    public void set_mass( float new_mass )
    {
        // update mass
        mass = new_mass;
        // update force/acceleration
        force = max_player_velocity * mass / (Time.deltaTime * (1.0f / kinetic_friction_constant - 1.0f));
        constant_acceleration = force / mass;
    }

    public void set_force( Vector3 new_force )
    {
        // update force/acceleration
        force = new_force;
        constant_acceleration = force / mass;
        // update max velocity
        max_player_velocity = constant_acceleration * Time.deltaTime * (1.0f / kinetic_friction_constant - 1.0f);
    }

    public void set_kinetic_friction( float new_frict )
    {
        // set new frict
        kinetic_friction_constant = new_frict;
        // update force/acceleration
        force = max_player_velocity * mass / (Time.deltaTime * (1.0f / kinetic_friction_constant - 1.0f));
        constant_acceleration = force / mass;
    }

    public void apply_impulse(Vector3 velocity, float impulse_mass)
    {
        const float impulse_time = 0.1f;
        Vector3 impulse_force = velocity * impulse_mass / impulse_time;
        player_velocity += impulse_force / mass;
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
    }

    private int signf( float val )
    {
        if (val == 0) return 0;
        else if (val > 0) return 1;
        else return -1;
    }
}

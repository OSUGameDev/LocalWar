using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MovementSys : NetworkBehaviour
{
    private Vector3 player_velocity;
    private Vector3 player_acceleration;
    public Vector3  min_speed;
    public Vector3  max_speed;
    public Vector3  acceleration;
    public Vector3  decceleration;
    private Vector3 previous_input_direction;
    private Vector3 input_direction;
    private Vector2 previous_mouse_axis;
    private Vector2 mouse_axis;
    public float sensitivity = 100.0f;

    private CharacterController kinematic_controller;

    private int jumps_used = 0;
    public int jump_count = 2;
    public float gravity = 9.8f;
    public float jump_speed = 10.0f;

    void Start()
    {
        min_speed = new Vector3(0.1f, 0.0f, 0.1f);
        max_speed = new Vector3(20.0f, 0.0f, 20.0f);
        acceleration = new Vector3(20.0f, 0.0f, 20.0f);
        decceleration = new Vector3(20.0f, 0.0f, 20.0f);

        player_velocity = min_speed;
        player_acceleration = new Vector3();
        input_direction = new Vector2();
        mouse_axis = new Vector2();

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

        // jumping:
        if (Input.GetButtonDown("Jump"))
        {
            player_velocity.y += jump_speed;
        }

        // movement:

        // get acceleration
        player_acceleration.x = 0.0f;
        player_acceleration.y = 0.0f; // -gravity * Time.deltaTime;
        player_acceleration.z = 0.0f;

        Vector3 movement_forward = new Vector3( transform.forward.x, 0.0f, transform.forward.z ); movement_forward.Normalize();
        // strafe
        if ( input_direction.x != 0 )
        {
            // accelerate left/right in regard to forward
            Vector3 cross = Vector3.Cross(movement_forward, transform.up);
            player_acceleration += cross * (float)( acceleration.x * input_direction.x );
        }
        else
        {
            // deccelerate based on velocity direction
            //Vector3 opposite_movement_direction = new Vector3( -1.0f * signf(player_velocity.x), 0.0f, -1.0f * signf(player_velocity.z) );
            //Vector3 cross = Vector3.Cross(opposite_movement_direction, transform.up);
            //player_acceleration += cross * decceleration.x;
            // static friction
            if (player_velocity.x > -min_speed.x && player_velocity.x < min_speed.x)
            {
                player_velocity.x = 0.0f;
                player_acceleration.x = 0.0f;
            }
        }

        // forward
        if (input_direction.z != 0.0f)
        {
            // accelerate with forward on x,z plane
            player_acceleration += movement_forward * acceleration.z * input_direction.z;
        }
        else
        {
            // deccelerate against velocity direction
            Vector3 velocity_direction = new Vector3(signf(player_velocity.x), 0.0f, signf(player_velocity.z)); velocity_direction.Normalize();
            player_acceleration -= velocity_direction * decceleration.z;
            // static friction
            if (player_velocity.z > -min_speed.z && player_velocity.z < min_speed.z)
            {
                player_velocity.z = 0.0f;
                player_acceleration.z = 0.0f;
            }
        }

        // apply speed constraints
        if (player_velocity.x < -max_speed.x || player_velocity.x > max_speed.x)
        {
            player_velocity.x = max_speed.x * signf(player_velocity.x);
        }
        if (player_velocity.z < -max_speed.z || player_velocity.z > max_speed.z)
        {
            player_velocity.z = max_speed.z * signf(player_velocity.z);
        }

        // apply acceleration
        player_velocity.x += player_acceleration.x * Time.deltaTime;
        player_velocity.y += player_acceleration.y * Time.deltaTime;
        player_velocity.z += player_acceleration.z * Time.deltaTime;

        Debug.Log(player_velocity);

        // apply movement
        Vector3 translation = player_velocity * Time.deltaTime;
        transform.Translate(translation, Space.World);
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
        Move();
    }

    private int signf( float val )
    {
        if (val == 0) return 0;
        else if (val > 0) return 1;
        else return -1;
    }
}

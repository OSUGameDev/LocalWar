using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MovementSys : NetworkBehaviour
{
    private Vector3 player_velocity;
    private Vector3 player_acceleration;
    public Vector3 min_speed = new Vector3(10.0f, 0.0f, 10.0f);
    public Vector3 max_speed = new Vector3(600.0f, 0.0f, 600.0f);
    public Vector3 acceleration = new Vector3(50.0f, 0.0f, 50.0f);
    public Vector3 decceleration = new Vector3(50.0f, 0.0f, 50.0f);
    private Vector2 previous_input_direction;
    private Vector2 input_direction;
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
        // TODO: add forward facing vector

        // movement
        input_direction.x = Mathf.Sign(Input.GetAxis("Horizontal"));
        input_direction.y = Mathf.Sign(Input.GetAxis("Vertical"));
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
        player_acceleration.y = 0.0f; // -gravity;
        player_acceleration.z = 0.0f;
        bool stop_strafe = true;
        bool stop_walk = true;

        // left
        if (input_direction.x < 0)
        {
            if (player_velocity.x <= min_speed.x && player_velocity.x > -max_speed.x)
            {
                player_acceleration.x = acceleration.x * input_direction.x;
                stop_strafe = false;
            }
        }
        // right
        else if (input_direction.x > 0)
        {
            if (player_velocity.x >= -min_speed.x && player_velocity.x < max_speed.x)
            {
                player_acceleration.x = acceleration.x * input_direction.x;
                stop_strafe = false;
            }
        }
        // forward
        if (input_direction.y < 0)
        {
            if (player_velocity.z <= min_speed.z && player_velocity.z > -max_speed.z)
            {
                player_acceleration.z = acceleration.z * input_direction.y;
                stop_walk = false;
            }
        }
        // back
        else if (input_direction.y > 0)
        {
            if (player_velocity.z >= -min_speed.z && player_velocity.z < max_speed.z)
            {
                player_acceleration.z = acceleration.z * input_direction.y;
                stop_walk = false;
            }
        }

        // stop strafe
        if (stop_strafe)
        {
            float v_sign = Mathf.Sign(player_velocity.x);
            float v_len = Mathf.Abs(player_velocity.x) - (decceleration.x * Time.deltaTime);
            if (v_len < 0)
            {
                v_len = 0;
            }
            player_velocity.x = v_sign * v_len;
        }
        // stop walk
        if (stop_walk)
        {
            float v_sign = Mathf.Sign(player_velocity.z);
            float v_len = Mathf.Abs(player_velocity.z) - (decceleration.z * Time.deltaTime);
            if (v_len < 0)
            {
                v_len = 0;
            }
            player_velocity.z = v_sign * v_len;
        }

        // apply acceleration
        player_velocity.x += player_acceleration.x * Time.deltaTime;
        player_velocity.y += player_acceleration.y * Time.deltaTime;
        player_velocity.z += player_acceleration.z * Time.deltaTime;

        // apply movement
        Debug.Log( player_velocity );
        kinematic_controller.Move(player_velocity);
    }

    void FixedUpdate()
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

}

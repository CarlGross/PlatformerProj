using System;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public enum PlayerState
    {
        Idle,
        Running,
        Jumping,
        Falling,
        WallSliding,
        AirStalling

    }
    public static PlayerState state;
    private float xInput = 0f;

    private bool space = false;

    private bool spaceUp = false;

    private bool action = false;

    private CharacterController controller;

    private bool sprint = false;

    private float verticalVelocity = -1f;

    private float airStallTimer = 0f;

    private float boost = 0f;

    public float speed = 10f;

    private float mult = 1f;

    public float gravity = -10f;

    private float time = 0f;

    public float wallVelocity = -2f;

    private float velocityInitial = -1f;

    public float jumpVelocity = 20f;

    private bool canAirStall = true;

    private float wallJumpBoost = 0f;

    private float wallJumpBoostTimer = 0f;

    private float leftWallCooldown = 0f;

    private float rightWallCooldown = 0f;

    private bool shortJump = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        Timer();
        Inputs();
        UpdateStatus();
        switch (state)
        {
            case PlayerState.Idle:
                HandleIdle();
                break;
            case PlayerState.Running:
                HandleRunning();
                break;
            case PlayerState.Jumping:
                HandleJumping();
                break;
            case PlayerState.Falling:
                HandleFalling();
                break;
            case PlayerState.WallSliding:
                HandleWallSliding();
                break;
            case PlayerState.AirStalling:
                HandleAirStalling();
                break;

        }
    }

    void UpdateStatus()
    {
        if (controller.isGrounded)
        {
            if (xInput == 0)
            {
                state = PlayerState.Idle;
            }
            else
            {
                state = PlayerState.Running;
            }
        }
        else if (airStallTimer > 0f)
        {
            state = PlayerState.AirStalling;
        }
        else if (verticalVelocity > 0f)
        {
            state = PlayerState.Jumping;
        }
        else if (IsTouchingWall() && ((leftWallCooldown <= 0 && LeftWall()) || (rightWallCooldown <= 0 && !LeftWall())))
        {
            state = PlayerState.WallSliding;
        }
        else
        {
            state = PlayerState.Falling;
        }

    }

    void Inputs()
    {
        xInput = Input.GetAxis("Horizontal");
        space = Input.GetKeyDown(KeyCode.Space);
        spaceUp = Input.GetKeyUp(KeyCode.Space);
        action = Input.GetKeyDown(KeyCode.E);
        sprint = Input.GetKey(KeyCode.LeftShift);
    }

    void HandleIdle()
    {
        shortJump = false;
        canAirStall = true;
        velocityInitial = 0f;
        time = 0f;
        verticalVelocity = -1f;
        if (space)
        {
            velocityInitial = jumpVelocity;
            verticalVelocity = velocityInitial;
        }
        controller.Move(new Vector3(0f, verticalVelocity * Time.deltaTime, 0f));
    }

    void Timer()
    {
        if (leftWallCooldown > 0f)
        {
            leftWallCooldown -= Time.deltaTime;
        }

        else if (rightWallCooldown > 0f)
        {
            rightWallCooldown -= Time.deltaTime;
        }

        if (boost > 0f)
        {
            boost -= Time.deltaTime;
        }

        if (wallJumpBoostTimer > 0f)
        {
            wallJumpBoostTimer -= Time.deltaTime;
        }
        else
        {
            wallJumpBoost = 0f;
        }


        if (airStallTimer > 0f)
        {
            airStallTimer -= Time.deltaTime;
            if (IsTouchingWall())
            {
                airStallTimer = 0f;
            }
        }

        else
        {
            time += Time.deltaTime;
        }
    }

    void HandleRunning()
    {
        shortJump = false;
        canAirStall = true;
        verticalVelocity = -1f;
        velocityInitial = 0f;
        time = 0f;
        if (space)
        {
            velocityInitial = jumpVelocity;
            verticalVelocity = velocityInitial;
        }
        if (action && boost <= 0f)
        {
            boost = 1f;
        }
        mult = 1f;
        if (sprint)
        {
            mult *= 2f;
        }
        if (boost > 0f)
        {
            mult *= 1.5f;
        }
        controller.Move(new Vector3((wallJumpBoost + xInput * speed * mult) * Time.deltaTime, verticalVelocity * Time.deltaTime, 0f));
    }

    void HandleAirStalling()
    {
        controller.Move(new Vector3((wallJumpBoost + xInput * speed * mult) * Time.deltaTime, 0f, 0f));
    }

    void ApplyGravity()
    {
        verticalVelocity = velocityInitial + gravity * time;
    }

    void HandleInAir()
    {

        if (canAirStall && action && airStallTimer <= 0f)
        {
            velocityInitial = 0f;
            time = 0f;
            airStallTimer = 0.3f;
            canAirStall = false;
        }

        ApplyGravity();

    }

    void HandleJumping()
    {
        if (spaceUp)
        {
            shortJump = true;
        }
        HandleInAir();
        if (shortJump)
        {
            verticalVelocity /= 2;
        }
        controller.Move(new Vector3((wallJumpBoost + xInput * speed * mult) * Time.deltaTime, verticalVelocity * Time.deltaTime, 0f));

    }

    void HandleFalling()
    {
        HandleInAir();
        controller.Move(new Vector3((wallJumpBoost + xInput * speed * mult) * Time.deltaTime, verticalVelocity * Time.deltaTime, 0f));
    }

    void HandleLeftWall()
    {

        rightWallCooldown = 0f;

        if (Input.GetAxis("Horizontal") < 0f)
        {
            time = 0f;
            velocityInitial = 0f;
        }
        if (space)
        {
            velocityInitial = jumpVelocity;
            wallJumpBoostTimer = 0.5f;
            wallJumpBoost = 10f;
            leftWallCooldown = 0.8f;
        }
        HandleInAir();
        if (Input.GetAxis("Horizontal") < 0f)
        {
            verticalVelocity = wallVelocity;
        }
        controller.Move(new Vector3((wallJumpBoost + xInput * speed * mult) * Time.deltaTime, verticalVelocity * Time.deltaTime, 0f));
    }

    void HandleRightWall()
    {

        leftWallCooldown = 0f;

        if (Input.GetAxis("Horizontal") > 0f)
        {
            time = 0f;
            velocityInitial = 0f;
        }
        if (space)
        {
            velocityInitial = jumpVelocity;
            wallJumpBoostTimer = 0.5f;
            wallJumpBoost = -10f;
            rightWallCooldown = 0.8f;
        }
        HandleInAir();
        if (Input.GetAxis("Horizontal") > 0f)
        {
            verticalVelocity = wallVelocity;
        }
        controller.Move(new Vector3((wallJumpBoost + xInput * speed * mult) * Time.deltaTime, verticalVelocity * Time.deltaTime, 0f));
    }

    void HandleWallSliding()
    {
        canAirStall = true;
        if (LeftWall())
        {
            HandleLeftWall();
        }
        else
        {
            HandleRightWall();
        }
    }


    bool IsTouchingWall()
    {
        return !controller.isGrounded && (Physics.Raycast(transform.position, new Vector3(-1f, 0f, 0f), 0.6f) || Physics.Raycast(transform.position, new Vector3(1f, 0f, 0f), 0.6f));
    }

    bool LeftWall()
    {
        return Physics.Raycast(transform.position, new Vector3(-1f, 0f, 0f), 0.6f);
    }
    


    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Enemy"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}

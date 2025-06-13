using System;
using System.Data;
// using System.Numerics;
using System.Security.Cryptography.X509Certificates;
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



    public Vector3 lastDirection = Vector3.right;
    public static PlayerState state;

    public float xInput = 0f;

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

    private float leftWallCooldown = 0f;

    private float rightWallCooldown = 0f;

    private bool shortJump = false;

    public float maxspeed = 20f;

    public float sprintSpeed = 1f;

    private float lastXInput = 0f;

    public float yInput;

    public float attackTime = 0f;

    public float attackCooldown = 0f;


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
        lastXInput = xInput;
        yInput = Input.GetAxis("Vertical");
        xInput = Input.GetAxis("Horizontal");
        space = Input.GetKeyDown(KeyCode.Space);
        spaceUp = Input.GetKeyUp(KeyCode.Space);
        action = Input.GetKeyDown(KeyCode.I);
        sprint = Input.GetKey(KeyCode.LeftShift);
        if (xInput != 0)
        {
            if (xInput > 0f)
            {
                lastDirection = Vector3.right;
            }
            else
            {
                lastDirection = Vector3.left;
            }
        }
       
    }

    void GeneralMove()
    {
        if (action && attackTime <= 0f && attackCooldown <= 0f)
        {
            attackCooldown = 1f;
            attackTime = 0.5f;
        }
        if (attackTime > 0f)
        {
            Attack();
        }
        print(attackTime);
        // hitbox.MoveHitBox();

        float xVelocity = xInput * speed * mult;
       // If you want to clamp max speed
        // if (Math.Abs(xVelocity) > maxspeed)
        // {
        //     if (xVelocity < 0)
        //     {
        //         xVelocity = -maxspeed;
        //     }
        //     else
        //     {
        //         xVelocity = maxspeed;
        //     }
        // }
         controller.Move(new Vector3(xVelocity * Time.deltaTime, verticalVelocity * Time.deltaTime, 0f));
    }

    void GroundActions()
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
        GeneralMove();
    }
    void HandleIdle()
    {
        GroundActions();
    }

    void Timer()
    {
        if (attackCooldown > 0f)
        {
            attackCooldown -= Time.deltaTime;
        }
        if (attackTime > 0f)
        {
            attackTime -= Time.deltaTime;
        }

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

    bool ChangeDirections()
    {
        if (xInput != lastXInput)
        {
            return true;
        }
        return false;
    }

    void HandleRunning()
    {
        
        if (ChangeDirections())
        {
            boost = 0f;
            sprintSpeed = 1f;
        }
        mult = 1f;
        if (action && sprintSpeed > 1f && boost <= 0f && attackCooldown <= 0f)
        {
            if (sprintSpeed < 2f)
            {
                sprintSpeed = 2.5f;
            }
            else
            {
                sprintSpeed = 3f;
            }
            boost = 0.5f;
        }
        if (sprint)
        {
            if (sprintSpeed < 2f)
            {
                sprintSpeed += Time.deltaTime * 4;
            }
            else if (sprintSpeed < 2.5f)
            {
                sprintSpeed += Time.deltaTime * 3;
            }
            else if (boost <= 0f)
            {
                if (sprintSpeed > 2.5f)
                {
                    sprintSpeed -= Time.deltaTime * 4;
                }
                
            }
            mult *= sprintSpeed;
        }
        else
        {
            if (sprintSpeed > 1f)
            {
                sprintSpeed -= Time.deltaTime * 4;
            }
            else
            {
                sprintSpeed = 1f;
            }
            mult *= sprintSpeed;
        }
        GroundActions();
    }

    void HandleAirStalling()
    {
        verticalVelocity = 0f;
        GeneralMove();
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
            verticalVelocity /= 3;
        }
        GeneralMove();

    }

    void HandleFalling()
    {
        HandleInAir();
        GeneralMove();
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
            mult = mult * 3f / sprintSpeed;
            sprintSpeed = 3f;
            leftWallCooldown = 0.7f;
        }
        HandleInAir();
        if (Input.GetAxis("Horizontal") < 0f)
        {
            verticalVelocity = wallVelocity;
        }
        GeneralMove();
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
            mult = mult * 3f / sprintSpeed;
            sprintSpeed = 3f;
            velocityInitial = jumpVelocity;
            rightWallCooldown = 0.7f;
        }
        HandleInAir();
        if (Input.GetAxis("Horizontal") > 0f)
        {
            verticalVelocity = wallVelocity;
        }
        GeneralMove();
    }

    void HandleWallSliding()
    {
        shortJump = false;
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

        if (Physics.Raycast(transform.position, new Vector3(-1f, 0f, 0f), out RaycastHit hitLeft, 0.6f))
        {
            if (hitLeft.transform.CompareTag("Terrain"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        if (Physics.Raycast(transform.position, new Vector3(1f, 0f, 0f), out RaycastHit hitRight, 0.6f))
        {
            if (hitRight.transform.CompareTag("Terrain"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }

    bool LeftWall()
    {
        return Physics.Raycast(transform.position, new Vector3(-1f, 0f, 0f), 0.6f);
    }


    void Attack()
    {
        Vector3 center;
        Vector3 boxSize = new(0.6f, 2f, 1f);
        Quaternion boxRot;
        if (yInput > 0)
        {
            boxRot = Quaternion.Euler(0f, 0f, 90f);
            center = transform.position + Vector3.up * 1.8f;
        }
         else if (yInput < 0)
        {
            boxRot = Quaternion.Euler(0f, 0f, 90f);
            center = transform.position + Vector3.down * 1.8f;
        }
        else
        {

            boxRot = Quaternion.identity;
            center = transform.position + lastDirection * 1.1f;
        }
        Collider[] hits = Physics.OverlapBox(center, boxSize / 2, boxRot);
        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                Destroy(hit.gameObject);
                canAirStall = true;
            }
        }  
    }


    private void OnDrawGizmos()
    {
        Vector3 center;
        Vector3 boxSize = new(0.6f, 2f, 1f);
        Quaternion boxRot;
        if (yInput > 0)
        {
            boxRot = Quaternion.Euler(0f, 0f, 90f);
            center = transform.position + Vector3.up * 1.8f;
        }
        else if (yInput < 0)
        {
            boxRot = Quaternion.Euler(0f, 0f, 90f);
            center = transform.position + Vector3.down * 1.8f;
        }
        else
        {

            boxRot = Quaternion.identity;
            center = transform.position + lastDirection * 1.1f;
        }
        Gizmos.color = Color.green;
        Gizmos.matrix = Matrix4x4.TRS(center, boxRot, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, boxSize);
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Enemy"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}

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
        Walking,
        Running,
        Jumping,
        Falling,
        WallSliding,
        WallGrinding,
        AirStalling,
        RingHanging

    }

    private GameObject ring;

    private bool onRing = false;

    public int health = 2;

    public bool activeController = true;

    public Vector3 lastDirection = Vector3.right;
    public static PlayerState state;

    public float xInput = 0f;

    private bool space = false;

    private bool spaceUp = false;

    private bool action = false;

    private CharacterController controller;

    private bool sprint = false;

    public float verticalVelocity = -1f;

    private float airStallTimer = 0f;

    private float boost = 0f;

    public float speed = 6f;

    public float mult = 1f;

    public float gravity = -110f;

    private float time = 0f;

    public float wallVelocity = -2f;

    private float velocityInitial = -1f;

    public const float jumpVelocity = 35f;

    private bool canAirStall = true;

    private bool shortJump = false;

    public float maxspeed = 20f;

    public float sprintSpeed = 1f;

    private float lastXInput = 0f;

    public float yInput;

    public float attackTime = 0f;

    public float attackCooldown = 0f;

    public float airStallCooldown = 0f;

    private int lastHealth;

    public UIControl ui;

    private bool grind = false;

    private float xLock = 1f;

    private float multLock = 2.5f;

    private float wallJumpTimer = 0f;

    private bool CheckLastX = false;

    private bool initBox = false;

    public Vector3 prevPos;


    void Start()
    {
        ui.UpdateHealthText();
        lastHealth = health;
        controller = GetComponent<CharacterController>();
    }

    void UIUpdate()
    {

        if (health != lastHealth)
        {
            ui.UpdateHealthText();
        }
        lastHealth = health;
    }

    void Update()
    {
        prevPos = transform.position;
        UIUpdate();
        Timer();
        Inputs();
        UpdateStatus();
        HandleState();
    }

    void UpdateStatus()
    {
        if (controller.isGrounded)
        {
            if (xInput == 0)
            {
                state = PlayerState.Idle;
            }
            else if (mult < 2f)
            {
                state = PlayerState.Walking;
            }
            else
            {
                state = PlayerState.Running;
            }

        }
        else if (onRing)
        {
            state = PlayerState.RingHanging;
        }
        else if (airStallTimer > 0f)
        {
            state = PlayerState.AirStalling;
        }
        else if (verticalVelocity > 0f)
        {
            state = PlayerState.Jumping;
        }
        else if (IsTouchingWall())
        {
            if (grind)
            {
                state = PlayerState.WallGrinding;
            }
            else
            {
                state = PlayerState.WallSliding;
            }
        }
        else
        {
            state = PlayerState.Falling;
        }
        print(state);

    }

    void HandleState()
    {
        switch (state)
        {
            case PlayerState.Idle:
                HandleIdle();
                break;
            case PlayerState.Walking:
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
            case PlayerState.WallGrinding:
                HandleWallSliding();
                break;
            case PlayerState.AirStalling:
                HandleAirStalling();
                break;
            case PlayerState.RingHanging:
                HandleRingHanging();
                break;
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

    void LockX()
    {
        CheckLastX = true;
        xInput = xLock;
        mult = multLock;
    }

    void HandleAttack()
    {
         if (action && attackTime <= 0f && attackCooldown <= 0f)
        {
            initBox = true;
            attackCooldown = 0.6f;
            attackTime = 0.3f;
        }
        if (attackTime > 0f)
        {
            Attack();
        }
    }
    void GeneralMove()
    {
        HandleAttack();

        if (boost > 0f || wallJumpTimer > 0f)
        {
            LockX();
        }
        else if (CheckLastX)
        {
            if (xInput != xLock)
            {
                mult = 1f;
                sprintSpeed = 1f;
            }
            CheckLastX = false;
        }



        float xVelocity = xInput * speed * mult;
       
        // Clamp max y speed
        if (Math.Abs(verticalVelocity) > maxspeed)
        {
            if (verticalVelocity < 0)
            {
                verticalVelocity = -maxspeed;
            }
            else
            {
                verticalVelocity = maxspeed;
            }
        }
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
        if (wallJumpTimer > 0f)
        {
            wallJumpTimer -= Time.deltaTime;
        }

        if (airStallCooldown > 0f)
        {
            airStallCooldown -= Time.deltaTime;
        }
        if (attackCooldown > 0f)
        {
            attackCooldown -= Time.deltaTime;
        }
        if (attackTime > 0f)
        {
            attackTime -= Time.deltaTime;
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
            multLock = sprintSpeed;
            xLock = xInput;
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

        if (canAirStall && action && airStallTimer <= 0f && airStallCooldown <= 0f)
        {
            airStallCooldown = 0.6f;
            StopJump();
            airStallTimer = 0.3f;
            canAirStall = false;
        }

        ApplyGravity();

    }

    bool CheckHead()
    {
        return Physics.Raycast(transform.position, Vector3.up, out RaycastHit hit, 1.3f) && hit.transform.CompareTag("Terrain");
    }

    void StopJump()
    {
        velocityInitial = 0f;
        time = 0f;
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
        if (CheckHead())
        {
            StopJump();
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
        if (Input.GetAxis("Horizontal") < 0f)
        {
            StopJump();
        }
        if (space)
        {
            wallJumpTimer = 0.5f;
            velocityInitial = jumpVelocity;
            sprintSpeed = 3f;
            xLock = 1f;
            multLock = 3f;
        }
        HandleInAir();
        if (Input.GetAxis("Horizontal") < 0f)
        {
            state = PlayerState.WallGrinding;
            verticalVelocity = wallVelocity;
            grind = true;
        }
      
    }

    void HandleRightWall()
    {
        if (Input.GetAxis("Horizontal") > 0f)
        {
            StopJump();
        }
        if (space)
        {
            wallJumpTimer = 0.5f;
            sprintSpeed = 3f;
            xLock = -1f;
            multLock = 3f;
            velocityInitial = jumpVelocity;
        }
        HandleInAir();
        if (Input.GetAxis("Horizontal") > 0f)
        {
            grind = true;
            verticalVelocity = wallVelocity;
        }
    }

    void HandleWallSliding()
    {
        grind = false;
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
        GeneralMove();
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

    Vector3 center;
    Vector3 boxSize;
    Quaternion boxRot;

    public enum AttackDir
    {
        Up,
        Down,
        Left,
        Right
    }

    public AttackDir dir;

    void InitializeBox()
    {
        boxSize = new(0.6f, 2f, 1f);
        if (yInput > 0)
        {
            dir = AttackDir.Up;
        }
        else if (yInput < 0)
        {
            dir = AttackDir.Down;
        }
        else if (lastDirection.Equals(Vector3.right))
        {
            dir = AttackDir.Right;
        }
        else
        {
            dir = AttackDir.Left;
        }
    }
    void Attack()
    {
        if (initBox)
        {
            InitializeBox();
            initBox = false;
        }
        if (dir == AttackDir.Up)
        {
            boxRot = Quaternion.Euler(0f, 0f, 90f);
            center = transform.position + Vector3.up * 1.8f;
        }
        else if (dir == AttackDir.Down)
        {
            boxRot = Quaternion.Euler(0f, 0f, 90f);
            center = transform.position + Vector3.down * 1.8f;
        }
        else if (dir == AttackDir.Right)
        {
            boxRot = Quaternion.identity;
            center = transform.position + Vector3.right * 1.1f;
        }
        else
        {
            boxRot = Quaternion.identity;
            center = transform.position + Vector3.left * 1.1f;
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
        if (attackTime > 0f)
        {
            Gizmos.color = Color.green;
            Gizmos.matrix = Matrix4x4.TRS(center, boxRot, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, boxSize);
        }

    }


    public void Bounce(float magnitude = jumpVelocity)
    {
        if (Input.GetKey(KeyCode.Space))
        {
            shortJump = false;
            spaceUp = false;
        }
        else
        {
            shortJump = true;
        }
        canAirStall = true;
        time = 0f;
        velocityInitial = magnitude;
    }

    void HandleRingHanging()
    {
        if (transform.position != ring.transform.position)
        {
            float ringSpeed = 10f * Time.deltaTime;
            Vector3 dir = (ring.transform.position - transform.position).normalized;
            controller.Move(ringSpeed * dir);
        }
        mult = 2.5f;
        HandleAttack();
        if (space)
        {
            Bounce();
            onRing = false;
        }
        else if (yInput < 0f)
        {
            onRing = false;
            StopJump();
        }
    }

    public void Damage(int dam)
    {
         if (health <= dam)
                {
                    SceneControl.resetScene();
                }
                else
                {
                    health -= dam;
                }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ring"))
        {
            onRing = true;
            ring = other.gameObject;
        }
    }

}

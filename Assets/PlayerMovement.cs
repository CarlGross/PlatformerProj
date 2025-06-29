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
        RingHanging,
        Plummeting

    }

    private GameObject ring;

    private bool onRing = false;

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

    public UIControl ui;

    private bool grind = false;

    private float xLock = 1f;

    private float multLock = 2.5f;

    private float wallJumpTimer = 0f;

    private bool CheckLastX = false;

    //JOSHUA_ADDEDCODE
    private Animator animator;
    private bool initBox = false;

    public Vector3 prevPos;

    private PlayerStats stats;


    void Start()
    {
        stats = GetComponent<PlayerStats>();
        controller = GetComponent<CharacterController>();
        //JOSHUA_ADDEDCODE
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        
        prevPos = transform.position;
        Timer();
        Inputs();
        UpdateStatus();
         SetAnimationState();
        HandleState();
    }

    //JOSHUA_ADDEDCODE
    void SetAnimationState()
    {

        animator.SetFloat("PlayerState", (float)state);
            // JOSHUA ADDED FOR SEAMLESS SYNC
        animator.SetFloat("mult", mult);
        animator.SetFloat("yvel", verticalVelocity);
    }

    void UpdateStatus()
    {

        if (controller.isGrounded)
        {
            if (xInput == 0)
            {
                state = PlayerState.Idle;
                //JOSHUA_ADDEDCODE
                //SetPlayerState(PlayerState.Idle);
            }
            else if (mult < 2f)
            {
                state = PlayerState.Walking;
                //JOSHUA_ADDEDCODE
                //SetPlayerState(PlayerState.Walking);
            }
            else
            {
                state = PlayerState.Running;
                //JOSHUA_ADDEDCODE
                //SetPlayerState(PlayerState.Running);
            }

        }
        else if (onRing)
        {
            state = PlayerState.RingHanging;
        }
        else if (plummet)
        {
            state = PlayerState.Plummeting;
        }
        else if (airStallTimer > 0f)
        {
            state = PlayerState.AirStalling;
            //JOSHUA_ADDEDCODE
            //SetPlayerState(PlayerState.AirStalling);
        }
        else if (verticalVelocity > 0f)
        {
            state = PlayerState.Jumping;
            //JOSHUA_ADDEDCODE
            //SetPlayerState(PlayerState.Jumping);
        }
        else if (IsTouchingWall())
        {
            if (grind)
            {
                state = PlayerState.WallGrinding;
                //JOSHUA_ADDEDCODE
                //SetPlayerState(PlayerState.WallGrinding);
            }
            else
            {
                state = PlayerState.WallSliding;
                //JOSHUA_ADDEDCODE
                //SetPlayerState(PlayerState.WallSliding);

            }
        }
        else
        {
            state = PlayerState.Falling;
            //JOSHUA_ADDEDCODE
            //SetPlayerState(PlayerState.Falling);
        }
        

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
            case PlayerState.Plummeting:
                HandlePlummeting();
                break;

        }
    }

    Vector3 direction;
    void Inputs()
    {
        lastXInput = xInput;
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
                direction = Vector3.right;
            }
            else
            {
                direction = Vector3.left;
            }
        }
        if (wallJumpTimer > 0f)
        {
            if (xLock == 1f)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
            else
            {
                transform.localScale = new Vector3(-1, 1, 1);
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
            attackCooldown = 0.5f;
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
        print(xInput);

        if (boost > 0f || wallJumpTimer > 0f)
        {
            LockX();
        }
        else if (CheckLastX)
        {
            if (direction.x != xLock)
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
        plummet = false;
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

    float TimeVar(float v)
    {
        if (v > 0f)
        {
            return v - Time.deltaTime;
        }
        else
        {
            return 0f;
        }
    }

    void Timer()
    {
        plummetCD = TimeVar(plummetCD);
        wallJumpTimer = TimeVar(wallJumpTimer);
        airStallCooldown = TimeVar(airStallCooldown);
        attackCooldown = TimeVar(attackCooldown);
        attackTime = TimeVar(attackTime);
        boost = TimeVar(boost);
        plummetDelay = TimeVar(plummetDelay);
        iFrames = TimeVar(iFrames);
        if (iFrames > 0f)
        {
            gameObject.GetComponent<SpriteRenderer>().color = Color.black;
        }
        else
        {
            gameObject.GetComponent<SpriteRenderer>().color = Color.white;
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
        if (direction != lastDirection)
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
        if (action && sprintSpeed <= 2.6f && sprintSpeed > 1f && boost <= 0f && attackCooldown <= 0f)
        {
            if (sprintSpeed < 2f)
            {
                sprintSpeed = 2.5f;
            }
            else
            {
                sprintSpeed = 3.5f;
            }
            boost = 0.4f;
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
    private bool plummet;
    void HandleInAir()
    {
        // Plummet Initiate
        if (space && yInput < 0 && plummetCD <= 0)
        {
            initBox = true;
            plummetDelay = 0.2f;
            plummet = true;
        }

        //Airstall Initiate
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
            //Remove first line if you want to have a down jump
            StopJump();
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
            //Remove first line if you want to have a down jump
            StopJump();
            wallJumpTimer = 0.5f;
            sprintSpeed = 3f;
            xLock = -1f;
            multLock = 3f;
            velocityInitial = jumpVelocity;
            // rightWallCooldown = 0.7f;
            
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
    public GameObject sword;
    private AttackAnimScript attackScript;
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
        attackScript = sword.GetComponent<AttackAnimScript>();
        attackScript.Swing(center - transform.position, boxRot);
        Collider[] hits = Physics.OverlapBox(center, boxSize / 2, boxRot);
        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                Destroy(hit.gameObject);
                canAirStall = true;
            }
            else if (hit.CompareTag("BreakableTerrain"))
            {
                Destroy(hit.gameObject);
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
    private float plummetDelay;

    private float plummetCD;
    void HandlePlummeting()
    {
        if (spaceUp)
        {
            plummetCD = 0.2f;
            plummet = false;
            StopJump();
        }
        else if (plummetDelay <= 0)
        {
            controller.Move(Vector3.down * Time.deltaTime * 30f);
            Attack();
        }
    }

    private float iFrames = 0f;
    public void Damage(int dam)
    {
        if (iFrames <= 0f)
        {
            stats.UpdateHealth(dam);
            iFrames = 1f;
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

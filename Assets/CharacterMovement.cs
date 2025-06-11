using System;
using System.Net.Http.Headers;
using System.Threading;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem.XInput;
using UnityEngine.SceneManagement;

public class CharacterMovement : MonoBehaviour
{
    private CharacterController controller;
    public float speed = 5f;

    public float gravity = -250f;

    private float time = 0f;

    public float jump = 65f;

    private float vI = 0f;

    public float sprint = 1.5f;

    private float sprintMult = 1f;

    private float airStall = 0f;

    private float airStallMult = 1f;

    private bool canAirStall = true;

    public float wallSpeed = -4f;

    private bool canWallFuck = true;

    private float xFactor = 0f;

    public float wallMult = 2f;
    float xInput = 0f;

    float wallNeg = 1f;

    void Start()
    {

        controller = GetComponent<CharacterController>();
        
    }

    // Update is called once per frame
    void Update()
    {
        xInput = Input.GetAxis("Horizontal");

        if (xFactor > 0.1f)
        {
            xFactor -= Time.deltaTime;
        }
        else if (xFactor < -0.1f)
        {
            xFactor += Time.deltaTime;
        }
        else
        {
            xFactor = 0f;
        }      

        if (airStall > 0f)
        {
            airStall -= Time.deltaTime;
            airStallMult = 0f;
            time = 0f;
            canAirStall = false;
        }
        else if (canWallFuck && wallFuck())
        {
            if (vI + 0.5f * gravity * time < 0)
            {
                vI = 0;
                time = 0f;
                wallSpeed = -4f;

            }
            else
            {
                time += Time.deltaTime;
                wallSpeed = 0;
            }


        }

        else
        {

            time += Time.deltaTime;

            airStallMult = 1f;
        }

        if (canWallFuck && wallFuck())
        {
            if (airStall > 0f)
            {
                vI = 0f;
                // time = 0f;
                airStallMult = 1f;
                airStall = 0f;
            }

            // vI = 0f;
            controller.Move(new Vector3(wallNeg * wallMult * xFactor * Time.deltaTime + Time.deltaTime * xInput * speed * sprintMult, wallSpeed * Time.deltaTime + Time.deltaTime * (vI + 0.5f * gravity * time), 0f));

            if (Input.GetKeyDown(KeyCode.Space))
            {
                canAirStall = true;
                wallNeg = 1f;
                if (!leftWallFuck())
                {
                    wallNeg = -1f;
                }
                xFactor = 0.8f;
                vI = jump;
                canWallFuck = false;

            }

        }
        else if (canAirStall)
        {

            controller.Move(new Vector3(wallNeg * wallMult * xFactor * Time.deltaTime + Time.deltaTime * xInput * speed * sprintMult, Time.deltaTime * (vI + 0.5f * gravity * time), 0f));
            canWallFuck = true;
        }
        else
        {

            controller.Move(new Vector3(wallNeg * wallMult * xFactor * Time.deltaTime + Time.deltaTime * xInput * speed * sprintMult, airStallMult * Time.deltaTime * (0.5f * gravity * time), 0f));
            canWallFuck = true;
        }



        if (controller.isGrounded)
        {
            canAirStall = true;
            vI = 0f;
            time = 0f;
            if (Input.GetKey(KeyCode.LeftShift))
            {
                sprintMult = sprint;
            }
            else
            {
                sprintMult = 1f;
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {

                vI = jump;
            }
            if (xFactor < 0.2f && Input.GetKeyDown(KeyCode.E))
            {
                xFactor = 0.8f;
                if (xInput < 0f)
                {
                    xFactor *= -1;
                }
            }

        }
        else
        {

            if (!wallFuck() && canAirStall && Input.GetKeyDown(KeyCode.E))
            {
                airStall = 0.35f;
            }
        }




    }
    void FixedUpdate()
    {

    }

   

    bool wallFuck()
    {
        return !controller.isGrounded && (Physics.Raycast(transform.position, new Vector3(-1f, 0f, 0f), 0.6f) || Physics.Raycast(transform.position, new Vector3(1f, 0f, 0f), 0.6f));
    }

    bool leftWallFuck()
    {
        return !controller.isGrounded && Physics.Raycast(transform.position, new Vector3(-1f, 0f, 0f), 0.6f);
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Enemy"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
   



}

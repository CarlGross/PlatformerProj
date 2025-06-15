using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    public float speed = 1f;
    public float runMult = 1.5f;
    public float gravity = -9.8f;

    public float jumpSpeed = 2f;

    private float timer = 0f;
    private bool jump = false;

    private bool checkGround = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.LeftShift)){
            speed *= runMult;
        }   
        if(Input.GetKeyUp(KeyCode.LeftShift)){
            speed /= runMult;
        }
        if(Input.GetKeyDown(KeyCode.Space)){
            if(this.GroundCheck()){
                jump = true;
            }
        }
    }

    void FixedUpdate()
    {
        if (jump){
            this.Jump();
        }
        this.transform.position += new Vector3(Input.GetAxis("Horizontal") * speed, 0f, 0f);
        if(!jump && !this.GroundCheck()){
            timer += Time.deltaTime;
            this.transform.position = new Vector3(this.transform.position.x, 0.5f * gravity*Mathf.Pow(timer, 2), 0f);
        }
        if (!jump && this.GroundCheck()){
            timer = 0f;
        }
       
    }

    bool GroundCheck(){
        if (Physics.Raycast(this.transform.position - new Vector3(0f, 0.3f, 0f), Vector3.down, 0.5f)){
            return true;
        }
        else{
            return false;
        }
    }

    

    void Jump(){
        timer += Time.deltaTime;
        checkGround = true;
        if (timer < 0.15f){
            checkGround = false;
        } 
        
        float yI = this.transform.position.y;
        if(!checkGround){
            this.transform.position = new Vector3(this.transform.position.x, yI + jumpSpeed*timer + 0.5f * gravity*Mathf.Pow(timer, 2), 0f);
        }
        else if (checkGround && !this.GroundCheck()){
        this.transform.position = new Vector3(this.transform.position.x, yI + jumpSpeed*timer + 0.5f * gravity*Mathf.Pow(timer, 2), 0f);
        }

        else {
        
            jump = false;
            timer = 0f;
            checkGround = true;
        }
    }
}

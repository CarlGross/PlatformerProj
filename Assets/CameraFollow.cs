using System.Data.Common;
using Unity.Collections;
using UnityEditor.SearchService;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject obj;
    private PlayerMovement playermov;
    private float Xoffset = 0f;
    private float currHeight;

    void Start()
    {
        currHeight = obj.transform.position.y + 2f;
        playermov = obj.GetComponent<PlayerMovement>();
    }
    void Update()
    {
        if (PlayerMovement.state != PlayerMovement.PlayerState.RingHanging)
        {


            float x = playermov.xInput;
            float speed = playermov.sprintSpeed;
            float playerHeight = obj.transform.position.y;

            if (x > 0f)
            {
                if (Xoffset < x * 4f)
                {
                    Xoffset += Time.deltaTime * speed;
                }
            }
            else if (x < 0f)
            {
                if (Xoffset > x * 4f)
                {
                    Xoffset -= Time.deltaTime * speed;
                }
            }

            if (currHeight + 10f < playerHeight)
            {
                currHeight += 40 * Time.deltaTime;
            }
            else if (currHeight + 6f < playerHeight)
            {
                currHeight += 12 * Time.deltaTime;
            }
            else if (currHeight + 3f < playerHeight)
            {
                currHeight += 2 * Time.deltaTime;
            }

            if (currHeight > playerHeight + 10f)
            {
                currHeight -= 40 * Time.deltaTime;
            }
            else if (currHeight > playerHeight + 6f)
            {
                currHeight -= 12 * Time.deltaTime;
            }
            else if (currHeight > playerHeight + 3f)
            {
                currHeight -= 2 * Time.deltaTime;
            }



            transform.position = new Vector3(obj.transform.position.x + Xoffset, currHeight, -10f);

        }
        
    }
}

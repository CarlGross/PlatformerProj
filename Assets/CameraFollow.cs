using Unity.Collections;
using UnityEditor.SearchService;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject obj;
    private PlayerMovement playermov;
    private float Xoffset = 0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playermov = obj.GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        float x = playermov.xInput;
        float speed = playermov.sprintSpeed;
        
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
        

        transform.position = new Vector3(obj.transform.position.x + Xoffset, 3f + obj.transform.position.y * 0.2f, -10f);
        
    }
}

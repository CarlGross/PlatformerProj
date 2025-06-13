using Unity.VisualScripting;
using UnityEngine;

public class playerHitbox : MonoBehaviour
{
    public GameObject player;
    private PlayerMovement movement;
   
    void Start()
    {
        movement = player.GetComponent<PlayerMovement>();
    }

   
    public void MoveHitBox()
    {
        float yInput = movement.yInput;
        Vector3 lastDirection = movement.lastDirection;
        if (yInput > 0)
        {
            transform.position = player.transform.position + Vector3.up * 2f;
        }
        else
        {
            transform.position = player.transform.position + lastDirection * 1.1f;
        }
    }

    public void Attack()
    {
    
    }
}

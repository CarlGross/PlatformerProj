using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{

    public bool bouncy = true;

    public int damage = 1;

    bool IsBounce(PlayerMovement player)
    {
        float lastFrameMove = player.verticalVelocity * Time.deltaTime;
        float playerPos = player.transform.position.y;
        float lastFrameHeight = playerPos - lastFrameMove;
        if (lastFrameHeight > transform.position.y)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    void OnTriggerEnter(Collider other)
{
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerMovement player = other.gameObject.GetComponent<PlayerMovement>();
            if (bouncy && IsBounce(player))
            {
                player.Bounce();
                Destroy(gameObject);
            }
            else
            {
                if (player.health <= damage)
                {
                    SceneControl.resetScene();
                }
                else
                {
                    player.health -= damage;
                }
            }
        }
}
}

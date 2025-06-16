using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using TreeEditor;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{

    public bool bouncy = true;

    public GameObject target;

    public bool follow = false;

    public bool canFloat = false;

    public bool boundlessfloat = false;

    public float speed = 8f;

    private float followDistance = 20f;

    public int damage = 1;

    private Vector3 direction;



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

    void Update()
    {
        if (follow)
        {
            Vector3 t = target.transform.position;
            Vector3 e = transform.position;
            float distance = (float) Math.Sqrt(Math.Pow(t.x - e.x, 2.0) + Math.Pow(t.y - e.y, 2.0));
            if (distance < followDistance)
            {
                if (canFloat)
                {
                    direction = (t - e).normalized;
                    if (!boundlessfloat && Physics.Raycast(transform.position, direction, out RaycastHit hit, 3f) && !hit.transform.CompareTag("Player"))
                    {
                        direction = Vector3.zero;
                    }
                }
                else
                {
                    float buffer = 1.1f;
                    direction = new Vector3(t.x - e.x, 0f, 0f).normalized;
                    if (!Physics.Raycast(transform.position + speed * Time.deltaTime * direction * buffer, Vector3.down, out RaycastHit hit, 1f) || hit.transform.CompareTag("Enemy"))
                    {
                        direction = Vector3.zero;
                    }
                }
                transform.position += speed * Time.deltaTime * direction;                
            }
        }
    }

    void OnTriggerEnter(Collider other)
{
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerMovement playerMove = other.gameObject.GetComponent<PlayerMovement>();
            if (bouncy && IsBounce(playerMove))
            {
                playerMove.Bounce();
                Destroy(gameObject);
            }
            else
            {
                if (playerMove.health <= damage)
                {
                    SceneControl.resetScene();
                }
                else
                {
                    playerMove.health -= damage;
                }
            }
        }
}
}

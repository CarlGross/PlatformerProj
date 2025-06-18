using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using TreeEditor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Android;

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

    public float jumpBuffer = 0f;



    bool IsBounce(PlayerMovement player)
    {
        int mask = LayerMask.GetMask("Enemy");
        return Physics.SphereCast(new(transform.position.x, player.prevPos.y + 1.2f, 0f), 1.1f, Vector3.down, out RaycastHit hit, 5f, mask);
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
                    if (!boundlessfloat && Physics.Raycast(transform.position, direction, out RaycastHit hit, 3f) && !hit.transform.CompareTag("Player") && !hit.transform.CompareTag("Ring")) 
                    {
                        direction = Vector3.zero;
                    }
                }
                else
                {
                    float buffer = 1.1f;
                    direction = new Vector3(t.x - e.x, 0f, 0f).normalized;
                    bool rayDown = !Physics.Raycast(transform.position + speed * Time.deltaTime * direction * buffer, Vector3.down, out RaycastHit hit, 1f) || hit.transform.CompareTag("Enemy");
                    bool rayForward = Physics.Raycast(transform.position, direction, out hit, 0.6f) && !hit.transform.CompareTag("Player");
                    if (rayDown || rayForward)
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

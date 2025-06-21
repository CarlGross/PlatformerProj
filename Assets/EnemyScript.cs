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

    public bool projectile = false;

    public float fireRate = 3f;

    public float fireSpeed = 5f;

    public GameObject projectilePrefab;

    private float fireCoolDown = 0f;

    public GameObject target;

    private Vector3 targetPos;

    public bool follow = false;

    public bool canFloat = false;

    public bool boundlessfloat = false;

    public float speed = 8f;

    private float followDistance = 20f;

    public int damage = 1;

    private Vector3 direction;



    void UpdatePos()
    {
        targetPos = target.transform.position;
    }


    bool IsBounce(PlayerMovement player)
    {
        int mask = LayerMask.GetMask("Enemy");
        return Physics.SphereCast(new(transform.position.x, player.prevPos.y + 1.2f, 0f), 1.1f, Vector3.down, out RaycastHit hit, 5f, mask);
    }

    bool CanSee()
    {
        int mask = LayerMask.GetMask("Player", "Terrain");
        return Physics.Raycast(transform.position, (targetPos - transform.position).normalized, out RaycastHit hit, followDistance, mask)
        && hit.transform.CompareTag("Player");
    }

    void Update()
    {
        
        
        if (projectile)
        {
            UpdatePos();
            Timer();
            if (fireCoolDown <= 0f && CanSee())
            {
                fireCoolDown = fireRate;
                Fire();
            }
        }
        if (follow)
        {
            UpdatePos();
            if (CanSee())
            {
                Vector3 t = targetPos;
                Vector3 e = transform.position;
                // float distance = (float)Math.Sqrt(Math.Pow(t.x - e.x, 2.0) + Math.Pow(t.y - e.y, 2.0));
                // float distance = Vector3.Distance(e, t);
                // if (distance < followDistance)
                // {
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
                // }
            }
        }
    }

    void Timer()
    {
        if (fireCoolDown > 0f)
        {
            fireCoolDown -= Time.deltaTime;
        }
    }

    void Fire()
    {
        GameObject obj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        obj.GetComponent<ProjectileMovement>().SetDir((target.transform.position - transform.position).normalized);
        obj.GetComponent<ProjectileMovement>().SetSpeed(fireSpeed);
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
                playerMove.Damage(damage);
            }
        }
}
}

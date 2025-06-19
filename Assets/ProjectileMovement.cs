using UnityEngine;

public class ProjectileMovement : MonoBehaviour
{
    public float speed = 4f;
    public float lifeSpan = 5f;

    public int damage = 1;

    private Vector3 dir;
    void Start()
    {
        Destroy(gameObject, lifeSpan);
    }

    public void SetDir(Vector3 newDir)
    {
        dir = newDir;
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }
    void Update()
    {
        transform.Translate(dir * speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerMovement>().Damage(damage);
            Destroy(gameObject);
        }
        else if (other.CompareTag("Terrain"))
        {
            Destroy(gameObject);
        }
       

    }
    
    
}

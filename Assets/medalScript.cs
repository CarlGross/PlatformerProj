using UnityEngine;

public class MedalScript : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerStats>().UpdateMedals();
            Destroy(gameObject);
        }
    }
}

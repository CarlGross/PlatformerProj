using UnityEditor.Scripting;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;

public class BouncePad : MonoBehaviour
{
    public float magnitude = 80f;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerMovement>().Bounce(magnitude);
        }
    }
}

using UnityEngine;

public class RopeScript : MonoBehaviour
{
    private bool space;

    private float xInput;

    private bool active = false;
    void Inputs()
    {
        xInput = Input.GetAxis("Horizontal");
        space = Input.GetKeyDown(KeyCode.Space);
    }

    

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            active = true;
        }
    }
}

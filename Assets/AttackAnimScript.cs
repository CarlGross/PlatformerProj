using UnityEngine;

public class AttackAnimScript : MonoBehaviour
{
    public GameObject target;
    private Vector3 swordOffSet = Vector3.zero;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = target.transform.position + swordOffSet;
    }
    public void Swing(Vector3 offset, Quaternion rot)
    {
        transform.rotation = rot;
        swordOffSet = offset;
        GetComponent<Animator>().ResetTrigger("Attack");
        GetComponent<Animator>().SetTrigger("Attack");
        GetComponent<Animator>().Play("SwordSingle");
    }
}

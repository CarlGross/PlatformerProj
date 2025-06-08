using UnityEditor.SearchService;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject obj;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(obj.transform.position.x, 3f + obj.transform.position.y * 0.2f, -10f);
        
    }
}

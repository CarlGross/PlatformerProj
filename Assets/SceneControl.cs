using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneControl : MonoBehaviour
{
    public static void resetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}

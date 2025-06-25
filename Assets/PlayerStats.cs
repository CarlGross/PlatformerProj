using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int health = 2;
    public int medals = 0;

    public GameObject uiObj;
    private UIControl ui;
    void Start()
    {
        ui = uiObj.GetComponent<UIControl>();
    }
    public void UpdateHealth(int dam)
    {
        if (health <= dam)
        {
            SceneControl.resetScene();
        }
        else
        {
            health -= dam;
        }
        ui.UpdateHealthText();
    }

    public void UpdateMedals()
    {
        medals++;
        ui.UpdateMedalText();
    }
}

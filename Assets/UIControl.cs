using TMPro;
using UnityEngine;

public class UIControl : MonoBehaviour
{
    public TMP_Text healthTextBox;
    public GameObject player;
    private PlayerStats stats;

    public TMP_Text medalTextBox;

    void Start()
    {
        stats = player.GetComponent<PlayerStats>();
        UpdateHealthText();
        UpdateMedalText();
    }
    public void UpdateHealthText()
    {

        healthTextBox.text = "Health: " + stats.health.ToString();
    }

    public void UpdateMedalText()
    {
        medalTextBox.text = "Medals: " + stats.medals.ToString();
    }

}

using TMPro;
using UnityEngine;

public class UIControl: MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Canvas canvas;
    public TMP_Text healthTextBox;
    public PlayerMovement player;


    // Update is called once per frame
    public void UpdateHealthText()
    {
       
        healthTextBox.text = "Health: " + player.health.ToString();
    }
}

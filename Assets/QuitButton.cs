using TMPro;
using UnityEngine;

public class QuitButton : MonoBehaviour
{
    public TextMeshProUGUI buttonText;
    public Color highlightColor;
    public Color defaultColor;
    public void QuitGame()
    {
        Debug.Log("Quit button pressed. Exiting game...");
        Application.Quit();
    }

    public void ChangeColor()
    {
       buttonText.color = highlightColor;
    }

    public void ResetColor()
    {
        buttonText.color = defaultColor;
    }
}

using UnityEngine;

public class ScreenEndOnEnable : MonoBehaviour
{
    public OutputManager om;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnEnable()
    {
        om.UpdateFinalOutputTextUI();
    }
}

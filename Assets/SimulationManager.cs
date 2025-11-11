using TMPro;
using UnityEngine;

public class SimulationManager : MonoBehaviour
{
    public int year = 0;

    public TMP_Text text_year;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddYear(int amt)
    {
        year += amt;
        text_year.text = year.ToString();
    }
}

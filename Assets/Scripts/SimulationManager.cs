using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SimulationManager : MonoBehaviour
{
    bool awaitingRestart;

    [Header("Data")]
    public int state_max;
    public int state_index;
    public int year_count = 0;

    public List<StateData> stateData;

    [Header("Text Objects")]
    public TMP_Text text_main;
    public TMP_Text text_year;

    [Header("Screens")]
    public GameObject screen_end_obj;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitializeSimulation();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddYear(int amt)
    {
        year_count += amt;
        text_year.text = year_count.ToString();
    }

    public void Advance()
    {
        if (awaitingRestart)
        {
            RestartSimulation();
            return;
        }

        bool isFinished = stateData[state_index].AdvanceState();
        if (isFinished)
        {
            NextState();
        }

        else
        {
            RetrieveMessage();
        }
    }

    void InitializeState()
    {
        stateData[state_index].InitState();
    }

    public void InitializeSimulation()
    {
        ResetData();
        InitializeState();
        RetrieveMessage();
    }

    public void NextState()
    {
        state_index++;

        if (state_index >= state_max)
        {
            EndSimulation();
        }

        else
        { 
            InitializeState();
        }

    }

    public void EndSimulation()
    {
        screen_end_obj.SetActive(true);
        awaitingRestart = true;
    }

    public void RestartSimulation()
    {
        ResetData();
        InitializeSimulation();
    }

    void RetrieveMessage()
    {
        string msg = stateData[state_index].RetrieveMessage();
        UpdateText(msg);
    }
    
    void UpdateText(string msg)
    {
        text_main.text = msg;
    }

    private void ResetData()
    {
        awaitingRestart = false;
        state_max = stateData.Count;
        state_index = 0;
        screen_end_obj.SetActive(false);
    }
}

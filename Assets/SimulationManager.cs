using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class SimulationManager : MonoBehaviour
{
    [Header("Data")]
    public Era currentEra;
    public EraEvent currentEvent;

    [Header("Draggables")]
    public EraManager eraManager;
    public TMP_Text text_mainField;
    public TMP_Text text_eraTitle;
    public TMP_Text text_next;
    public TMP_Text text_optionA;
    public TMP_Text text_optionB;
    public TMP_Text text_end;
    public GameObject screen_end;
    public Slider slider;

    public delegate void FunctionDelegate();
    Queue<FunctionDelegate> actionQueue = new();
    Tuple<string, int> outcomeData;

    public void Advance()
    {
        if (actionQueue.Count > 0)
        {
            FunctionDelegate func = actionQueue.Dequeue();
            func();
        }
    }

    public void OptionA()
    {
        outcomeData = currentEvent.GetOptionAOutcome();
        Advance();
    }

    public void OptionB()
    {
        outcomeData = currentEvent.GetOptionBOutcome();
        Advance();
    }

    void LoadEraData()
    {
        ClearUIText();

        currentEra = eraManager.GetRandomEraObj();
        currentEvent = currentEra.GetRandomEvent();
        text_eraTitle.text = currentEra.GetEraName();
        text_optionA.text = currentEvent.GetOptionAText();
        text_optionB.text = currentEvent.GetOptionBText();

        // Enqueue Actions
        actionQueue.Enqueue(DisplayIntroMessage);
        actionQueue.Enqueue(DisplayAction);
        actionQueue.Enqueue(DisplayOutcome);
        actionQueue.Enqueue(LoadEraData);

        // Display Era Title
        DisplayEraTitle(true);
        Invoke(nameof(DisappearingTitle), 1);
    }

    // ACTIONS
    void DisplayIntroMessage()
    {
        text_mainField.text = currentEvent.GetIntroMessage();
    }

    void DisplayAction()
    {
        text_mainField.text = currentEvent.GetActionMessage();
        DisplayButtonOptions(true);
        DisplayButtonNext(false);
    }
    void DisplayOutcome()
    {
        DisplayButtonOptions(false);
        DisplayButtonNext(true);
        SetOutcome();
    }

    void SetOutcome()
    {
        text_mainField.text = outcomeData.Item1;
        ModifySlider(outcomeData.Item2);
    }

    // UI SECTION
    void DisappearingTitle()
    {
        DisplayEraTitle(false);
    }

    void DisplayEraTitle(bool active)
    {
        text_eraTitle.gameObject.SetActive(active);
    }

    void DisplayButtonOptions(bool active)
    {
        text_optionA.transform.parent.gameObject.SetActive(active);
        text_optionB.transform.parent.gameObject.SetActive(active);
    }

    void DisplayButtonNext(bool active)
    {
        text_next.transform.parent.gameObject.SetActive(active);
    }

    void DisplayScreenEnd(bool active)
    {
        screen_end.SetActive(active);
    }

    void ModifySlider(int val)
    {
        slider.value += val;
        slider.value = Mathf.Clamp(slider.value, 0, 10);
        if (slider.value <= 0)
        {
            EndSimulation(false);
        }

        else if (slider.value >= 10)
        {
            EndSimulation(true);
        }
    }
    
    void EndSimulation(bool win)
    {
        DisplayScreenEnd(true);
        text_end.text = win ? "Win" : "Lose";
    }
    public void RestartSimulation()
    {
        DisplayScreenEnd(false);
        actionQueue.Clear();
        slider.value = 5;
        LoadEraData();
    }
    void ClearUIText()
    {
        text_eraTitle.text = "";
        text_optionA.text = "";
        text_optionB.text = "";
        text_mainField.text = "";
        text_end.text = "";
    }

    private void Start()
    {
        LoadEraData();
    }


}

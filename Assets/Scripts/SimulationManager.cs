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
    public CityManager cityManager;

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

    [Header("Era Transition Settings")]
    public int eventsPerEra = 5;
    private int eventCounter = 0;
    private int eraIndex = 0;

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

    public void Options(int index)
    {
        outcomeData = currentEvent.GetOptionOutcome(index);
        Advance();
    }

    void LoadEraData()
    {
        ClearUIText();

        if (actionQueue.Count != 0)
        {
            eventCounter++;
        }

        if (eventCounter >= eventsPerEra)
        {
            AdvanceEra();
            return;
        }

        // old code
        //currentEra = eraManager.GetRandomEraObj();
        
        // new code to get the current era
        currentEra = eraManager.GetEraObj(eraIndex);

        currentEvent = currentEra.GetRandomEvent();
        text_eraTitle.text = currentEra.GetEraName();

        // Enqueue Actions
        actionQueue.Enqueue(DisplayIntroMessage);
        actionQueue.Enqueue(DisplayAction);
        actionQueue.Enqueue(DisplayOutcome);
        actionQueue.Enqueue(LoadEraData);

        // Display Era Title
        DisplayEraTitle(true);
        Invoke(nameof(DisappearingTitle), 1);
    }

    // new function to transit to the next era
    void AdvanceEra()
    {
        eraIndex++;
        eventCounter = 0;

        if (eraIndex >= eraManager.eraObjs.Count)
        {
            EndSimulation(true);
            return;
        }

        Debug.Log($"Transitioning to Era {eraIndex + 1}: {eraManager.eraObjs[eraIndex].GetEraName()}");
        LoadEraData();
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

        // added code for transition to next era
        eraIndex = 0;
        eventCounter = 0;

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

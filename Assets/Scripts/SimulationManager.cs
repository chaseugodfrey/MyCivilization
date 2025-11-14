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
    public TMP_Text text_eraHeader;
    public TMP_Text text_eraTitle;
    public TMP_Text text_next;
    public TMP_Text text_optionA;
    public TMP_Text text_optionB;
    public TMP_Text text_end;
    public GameObject screen_end;
    public Slider slider;
    public GameObject mainField;
    public Scrollbar verticalScrollbar;
    public GameObject nextButton;
    public GameObject optionA;
    public GameObject optionB;

    public delegate void FunctionDelegate();
    Queue<FunctionDelegate> actionQueue = new();
    Tuple<string, int> outcomeData;
    private int eventCounter = 0;
    private int eraCounter = 0;


    public void Advance()
    {
        if (actionQueue.Count > 0)
        {
            FunctionDelegate func = actionQueue.Dequeue();
            func();
            verticalScrollbar.value = 1;
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

        currentEra = eraManager.GetEraObj(eraCounter);
        currentEvent = currentEra.GetRandomEvent();
        text_eraTitle.text = currentEra.GetEraName();
        text_eraHeader.text = currentEra.GetEraName();
        cityManager.UpdateProsperityUI();

        // Enqueue Actions
        actionQueue.Enqueue(DisplayIntroMessage);
        actionQueue.Enqueue(DisplayAction);
        actionQueue.Enqueue(DisplayOutcome);
        actionQueue.Enqueue(EventCount);
        actionQueue.Enqueue(LoadEraData);

        // Display Era Title
        DisplayEraTitle(true);
        Invoke(nameof(DisappearingTitle), 1);
    }

    // ACTIONS

    void EventCount()
    {
        ++eventCounter;
        if (eventCounter >=2)
        {
            eventCounter = 0;
            ++eraCounter;
        }
    }
    void DisplayIntroMessage()
    {
        slider.gameObject.SetActive(true);
        text_mainField.text = currentEvent.GetIntroMessage();
        DisplayButtonOptions(false);
        DisplayButtonNext(true);
    }

    void DisplayAction()
    {
        text_mainField.text = currentEvent.GetActionMessage();
        text_optionA.text = currentEvent.GetOptionsText().Item1;
        text_optionB.text = currentEvent.GetOptionsText().Item2;
        DisplayButtonOptions(true);
        DisplayButtonNext(false);
    }

    public void OptionPressed(int index)
    {
        if (index == 0)
        {
            outcomeData = currentEvent.GetOptionOutcome(currentEvent.leftIndex);
        }
        else
        {
            outcomeData = currentEvent.GetOptionOutcome(currentEvent.rightIndex);
        }            
        Advance();
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

    void DisplayTheEnd()
    {
        text_end.text = "The End";
        DisplayScreenEnd(true);
        DisplayButtonNext(false);
    }
    void DisappearingTitle()
    {
        DisplayEraTitle(false);
        mainField.SetActive(true);
        nextButton.SetActive(true);
        Advance();
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
        cityManager.ModifyProsperity(val);
        slider.value = Mathf.Clamp(slider.value, 0, 20);
        if (slider.value <= 0)
        {
            EndSimulation(false);
        }

        //else if (slider.value >= 20)
        //{
        //    EndSimulation(true);
        //}
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
        cityManager.ActiveCity = new CityManager.City();
        cityManager.ActiveCity.Prosperity = 10;
    }


}

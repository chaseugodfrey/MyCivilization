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
    public TMP_Text text_cityName;
    public GameObject screen_end;
    public Slider slider;
    public GameObject mainField;
    public Scrollbar verticalScrollbar;
    public GameObject nextButton;
    public GameObject optionA;
    public GameObject optionB;
    public GameObject nameCity;

    public delegate void FunctionDelegate();
    Queue<FunctionDelegate> actionQueue = new();
    Tuple<string, int> outcomeData;
    private int eventCounter = 0;
    private int eraCounter = 0;

    bool newEra = true;
    bool cityNamed = false;
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
        if(eraCounter >= 5)
        {
            DisplayTheEnd();
            return;
        }

        ClearUIText();

        currentEra = eraManager.GetEraObj(eraCounter);
        currentEvent = currentEra.GetRandomEvent();
        text_eraTitle.text = currentEra.GetEraName();
        text_eraHeader.text = currentEra.GetEraName();
        cityManager.UpdateProsperityUI();

        actionQueue.Enqueue(DisplayTitle);
        actionQueue.Enqueue(DisplayIntroMessage);
        actionQueue.Enqueue(DisplayAction);
        actionQueue.Enqueue(DisplayOutcome);
        actionQueue.Enqueue(EventCount);
        actionQueue.Enqueue(LoadEraData);

        if (cityNamed)
        {
            Advance();
        }
        else
        {
            NameCity();
        }
    }

    // ACTIONS
    void NameCity()
    {
        nameCity.SetActive(true);
    }

    public void CityNamed()
    {
        cityNamed = true;
        cityManager.ActiveCity.Name = nameCity.GetComponentInChildren<TMP_InputField>().text;
        text_cityName.text = cityManager.ActiveCity.Name;
        nameCity.SetActive(false);
        newEra = true;
        Invoke(nameof(DisappearingTitle), 1);
    }

    void EventCount()
    {
        ++eventCounter;
        if (eventCounter >=2)
        {
            eventCounter = 0;
            ++eraCounter;
            newEra = true;
        }
        Advance();
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
        Tuple<string, string> leagueOfLegends = currentEvent.GetOptionsText();
        text_optionA.text = leagueOfLegends.Item1;
        text_optionB.text = leagueOfLegends.Item2;
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

    void DisplayTitle()
    {
        if (newEra)
        {
            newEra = false;
            mainField.SetActive(false);
            DisplayEraTitle(true);
            nextButton.SetActive(false);
            Invoke(nameof(DisappearingTitle), 1);
        }
        else
        {
            Advance();
        }

    }
    void DisappearingTitle()
    {
        DisplayEraTitle(false);
        mainField.SetActive(true);
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
        slider.value = Mathf.Clamp(slider.value, slider.minValue, slider.maxValue);
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
        cityNamed = false;
        cityManager.ActiveCity.Name = "";
        DisplayScreenEnd(false);
        actionQueue.Clear();
        cityManager.ActiveCity.Prosperity = 50;
        cityManager.UpdateProsperityUI();
        eraCounter = 0;
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
        cityManager.ActiveCity = new CityManager.City();
        cityManager.ActiveCity.Prosperity = 50;
        cityManager.UpdateProsperityUI();
    }


}

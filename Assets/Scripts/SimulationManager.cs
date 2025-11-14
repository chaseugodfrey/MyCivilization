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
    public OutputManager outputManager;
    public TMP_Text text_mainField;
    public TMP_Text text_eraHeader;
    public TMP_Text text_eraTitle;
    public TMP_Text text_eraDescription;
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

    [Header("Debug")]
    public int eventCounter = 0;
    public int eraCounter = 0;

    public bool newEra = true;
    public bool cityNamed = false;
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
        Debug.Log("Load era Data");
        if (eraCounter >= 5)
        {
            DisplayTheEnd();
            return;
        }

        ClearUIText();

        currentEra = eraManager.GetEraObj(eraCounter);
        currentEvent = currentEra.GetRandomEvent();

        string eraName = currentEra.GetEraName();
        Debug.Log("Era Name:" + eraName);
        string eraDescription = currentEra.GetEraDescription();
        text_eraTitle.text = eraName;
        text_eraDescription.text = eraDescription;
        text_eraHeader.text = eraName;
        cityManager.UpdateProsperityUI();

        actionQueue.Enqueue(DisplayTitle);
        //actionQueue.Enqueue(DisplayIntroMessage);
        actionQueue.Enqueue(DisplayAction);
        actionQueue.Enqueue(DisplayOutcome);
        actionQueue.Enqueue(EventCount);
        actionQueue.Enqueue(LoadEraData);

        if (cityNamed)
        {
            //Advance();
        }
        
        else
        {
            NameCity();
        }

        if(eventCounter != 0 || eraCounter != 0)
        {
            Advance();
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
        Debug.LogWarning("This is happening too late?");
        SetManagerCityName(cityManager.ActiveCity.Name);
        nameCity.SetActive(false);
        newEra = true;
        Advance();
        //Invoke(nameof(DisappearingTitle), 5);
        //Debug.LogError("This is happening too late 2?");
    }

    public void SetManagerCityName(string cityName)
    {
        text_cityName.text = cityName;
        foreach (var era in eraManager.eraObjs)
        {
            foreach (var eraEvent in era.mEraEvents)
            {
                eraEvent.SetActionMessage(cityName);
            }
            era.SetEraDescription(cityName);
        }
        outputManager.cityName = cityName;
    }

    void EventCount()
    {
        Debug.Log("EventCount");
        ++eventCounter;
        if (eventCounter >=2)
        {
            eventCounter = 0;
            ++eraCounter;
            newEra = true;
        }
        DisplayMainField(false);
        DisplayEraTitle(true);
        Advance();
    }
    void DisplayIntroMessage()
    {
        Debug.Log("Display Intro Message");
        slider.gameObject.SetActive(true);
        string msg = currentEvent.GetIntroMessage();
        text_mainField.text = msg;
        outputManager.AddOutputMessage(msg);
        DisplayButtonOptions(false);
        DisplayButtonNext(true);
    }

    void DisplayAction()
    {
        Debug.Log("Display Action");
        DisplayEraTitle(false);
        DisplayMainField(true);
        text_mainField.text = currentEvent.GetActionMessage();
        outputManager.AddOutputMessage("Event: " + currentEvent.GetActionMessage() + "\n");
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
            outputManager.AddOutputMessage(text_optionA.text + "\n");
            outcomeData = currentEvent.GetOptionOutcome(currentEvent.leftIndex);
        }
        else
        {
            outputManager.AddOutputMessage(text_optionB.text + "\n");
            outcomeData = currentEvent.GetOptionOutcome(currentEvent.rightIndex);
        }

        Advance();
    }

    void DisplayOutcome()
    {
        Debug.Log("Display Outcome");
        DisplayButtonOptions(false);
        DisplayButtonNext(true);
        SetOutcome();
        cityManager.LogHistoryEntry(currentEra, currentEvent,currentEvent.rolledOutcome);
    }

    void SetOutcome()
    {
        text_mainField.text = outcomeData.Item1;
        outputManager.AddOutputMessage("Outcome: " + outcomeData.Item1 + "\n" + "\n");
        ModifySlider(outcomeData.Item2);
    }

    // UI SECTION

    void DisplayTheEnd()
    {
        text_end.text = "The End";
        DisplayScreenEnd(true);
        DisplayButtonNext(false);
        outputManager.CreateOutputFile();
    }

    void DisplayTitle()
    {
        Debug.Log("Display Title");
        if (newEra)
        {
            //newEra = false;
            mainField.SetActive(false);
            DisplayEraTitle(true);
            Debug.LogWarning(cityManager.ActiveCity.Name + "is THISSS");
            string eraDescription = currentEra.GetEraDescription();
            text_eraDescription.text = eraDescription;
            Debug.LogWarning(cityManager.ActiveCity.Name + "is THISSS2");
            //DisplayEraDescription(true);
            nextButton.SetActive(true);
           //Invoke(nameof(DisappearingTitle), 1);
        }
        if (eventCounter == 0)
        {
            outputManager.AddOutputMessage("[" + currentEra.GetEraName() +" Era" + "]");
            outputManager.AddOutputMessage(currentEra.GetEraDescription() + "\n" + "\n");
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
    void DisplayEraDescription(bool active)
    {
        text_eraDescription.gameObject.SetActive(active);
    }

    void DisplayMainField(bool active)
    {
        mainField.gameObject.SetActive(active);
        text_mainField.gameObject.SetActive(active);
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
        cityManager.history.Clear();
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
        text_eraDescription.text = "";
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

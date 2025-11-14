using System.Collections.Generic;
using System.IO.Enumeration;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class CityManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Slider prosperitySlider;
    public City mActiveCity;

    [Header("History")]
    [SerializeField] public List<EventHistoryEntry> history = new();

    [System.Serializable]
    public class City
    {
        public string mName = "";
        public int mProsperity = 0;

        public string Name
        {
            get => mName;
            set => mName = value;
        }

        public int Prosperity
        {
            get => mProsperity;
            set => mProsperity = value;
        }
    }


    public City ActiveCity
    {
        get => mActiveCity;
        set => mActiveCity = value;
    }

    public void UpdateProsperityUI()
    {
        if (prosperitySlider != null && mActiveCity != null)
        {
            prosperitySlider.value = mActiveCity.Prosperity;
        }
    }

    public void ModifyProsperity(int delta)
    {
        if (mActiveCity == null) return;

        mActiveCity.Prosperity += delta;
        UpdateProsperityUI();
    }

    [System.Serializable]
    public class EventHistoryEntry
    {
        //public int eraIndex;          // which era number (0–4)
        //public int eventIndexInEra;   // 0 or 1 within that era

        public Era era;               // ScriptableObject reference
        public EraEvent eraEvent;     // ScriptableObject reference

        // Player choice
        public int chosenOptionIndex; // 0 = left, 1 = right (or use your own mapping)
        public int finalOutcome;

        // Outcome
        //public string outcomeText;
        //public int prosperityBefore;
        //public int prosperityAfter;
        //public int prosperityDelta;   // same as outcomeData.Item2
    }

    public void LogHistoryEntry(Era currentEra, EraEvent currentEraEvent, int rolledOutcome)
    {
        var entry = new EventHistoryEntry
        {
            era = currentEra,
            eraEvent = currentEraEvent,
            finalOutcome = rolledOutcome
        };

        history.Add(entry);
    }

    
}

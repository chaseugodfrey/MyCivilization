using JetBrains.Annotations;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EraEvent", menuName = "Scriptable Objects/EraEvent")]
public class EraEvent : ScriptableObject
{
    [SerializeField] string mIntroMessage;
    [SerializeField] string mActionMessage;    
    [SerializeField] string mProcessedActionMessage;    

    [SerializeField]public List<string> mOptionMessages;
    [SerializeField]public List<string> mProcessedOptionMessages;
    [SerializeField]public List<string> mOutcomeMessages;
    [SerializeField]public List<string> mProcessedOutcomeMessages;
    [SerializeField] List<int> mOutcomeValues;
    [SerializeField] List<int> mStableValues;

    public int leftIndex;
    public int rightIndex;
    public int rolledOutcome;

    public string GetEraDescription()
    {
        return mIntroMessage;
    }

    public void SetEraDescription(string cityName)
    {
        string processed = mIntroMessage.Replace("<CityName>", cityName);
        mIntroMessage = processed;
    }
    public string GetIntroMessage()
    {
        return mIntroMessage;
    }

    public string GetActionMessage()
    {
        return mProcessedActionMessage;
    }

    public void SetActionMessage(string cityName)
    {
        mProcessedActionMessage = mActionMessage.Replace("<CityName>", cityName);
    }

    public Tuple<string, string> GetOptionsText()
    {
        if (mOptionMessages == null || mOptionMessages.Count < 2)
        {
            Debug.LogWarning("Not enough options to choose from!");
            return new Tuple<string, string>("N/A", "N/A");
        }

        leftIndex = UnityEngine.Random.Range(0, mOptionMessages.Count);
        rightIndex = leftIndex;

        // ensure indexB is different
        do
        {
            rightIndex = UnityEngine.Random.Range(0, mOptionMessages.Count);
        } while (rightIndex == leftIndex);

        string optionA = mProcessedOptionMessages[leftIndex];
        string optionB = mProcessedOptionMessages[rightIndex];
        Debug.Log($"Option A index: {leftIndex}, Option B index: {rightIndex}");

        return new Tuple<string, string>(optionA, optionB);
    }

    public Tuple<string, int> GetOptionOutcome(int index)
    {
        int roll = UnityEngine.Random.Range(index * 2, index * 2 + 2);
        rolledOutcome = roll;
        Debug.Log("Index Option: " + index);
        Debug.Log("Rolled Outcome: " + roll);
        return new Tuple<string, int>(mProcessedOutcomeMessages[roll], mOutcomeValues[roll]);
    }

    public void SetCityNameForEverything(string cityName)
    {
        mProcessedOptionMessages.Clear();
        mProcessedOutcomeMessages.Clear();
        SetActionMessage(cityName);

        foreach(var option in mOptionMessages)
        {
            mProcessedOptionMessages.Add(option.Replace("<CityName>", cityName));
        }
        foreach (var outcome in mOutcomeMessages)
        {
            mProcessedOutcomeMessages.Add(outcome.Replace("<CityName>", cityName));
        }
    }
}

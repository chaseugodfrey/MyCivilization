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

    [SerializeField] List<string> mOptionMessages;
    [SerializeField] List<string> mOutcomeMessages;
    [SerializeField] List<int> mOutcomeValues;
    [SerializeField] List<int> mStableValues;

    public int leftIndex;
    public int rightIndex;
    public int rolledOutcome;

    public string GetEraDescription()
    {
        return mIntroMessage;
    }
    public string GetIntroMessage()
    {
        return mIntroMessage;
    }

    public string GetActionMessage()
    {
        return mActionMessage;
    }

    public Tuple<string, string> GetOptionsText()
    {
        if (mOptionMessages == null || mOptionMessages.Count < 2)
        {
            Debug.LogWarning("Not enough options to choose from!");
            return new Tuple<string, string>("N/A", "N/A");
        }

        int leftIndex = UnityEngine.Random.Range(0, mOptionMessages.Count);
        int rightIndex = leftIndex;

        // ensure indexB is different
        do
        {
            rightIndex = UnityEngine.Random.Range(0, mOptionMessages.Count);
        } while (rightIndex == leftIndex);

        string optionA = mOptionMessages[leftIndex];
        string optionB = mOptionMessages[rightIndex];
        Debug.Log($"Option A index: {leftIndex}, Option B index: {rightIndex}");

        return new Tuple<string, string>(optionA, optionB);
    }

    public Tuple<string, int> GetOptionOutcome(int index)
    {
        int roll = UnityEngine.Random.Range(index * 2, index * 2 + 2);
        rolledOutcome = roll;
        return new Tuple<string, int>(mOutcomeMessages[roll], mOutcomeValues[roll]);
    }
}

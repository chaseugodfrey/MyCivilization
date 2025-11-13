using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EraEvent", menuName = "Scriptable Objects/EraEvent")]
public class EraEvent : ScriptableObject
{
    [SerializeField] string mIntroMessage;
    [SerializeField] string mActionMessage;
    [SerializeField] string mOptionAText;
    [SerializeField] string mOptionBText;
    [SerializeField] List<string> mOutcomeAMessages;
    [SerializeField] List<int> mOutcomeAValues;
    [SerializeField] List<string> mOutcomeBMessages;
    [SerializeField] List<int> mOutcomeBValues;

    public string GetIntroMessage()
    {
        return mIntroMessage;
    }
    public string GetActionMessage()
    {
        return mActionMessage;
    }

    public string GetOptionAText()
    {
        return mOptionAText;
    }
    public string GetOptionBText()
    {
        return mOptionBText;
    }

    public Tuple<string, int> GetOptionAOutcome()
    {
        int roll = UnityEngine.Random.Range(0, mOutcomeAMessages.Count);
        return new Tuple<string, int>(mOutcomeAMessages[roll], mOutcomeAValues[roll]);
    }

    public Tuple<string, int> GetOptionBOutcome()
    {
        int roll = UnityEngine.Random.Range(0, mOutcomeBMessages.Count);
        return new Tuple<string, int>(mOutcomeBMessages[roll], mOutcomeBValues[roll]);
    }
}

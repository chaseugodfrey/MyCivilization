using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NarrationData", menuName = "Data/Narration Data")]
public class NarrationData : ScriptableObject
{
    [Header("Positive Option Prefixes (for player choices)")]
    [SerializeField] public List<string> positiveOptionPrefixes;

    [Header("Negative Option Prefixes (for player choices)")]
    [SerializeField] public List<string> negativeOptionPrefixes;

    [Header("Event Introduction Prefixes")]
    [Tooltip("Used to introduce events more narratively")]
    [SerializeField] public List<string> eventIntros = new List<string>
    {
        "As days passed, a new challenge emerged: ",
        "The winds of change brought forth an unexpected turn: ",
        "In the midst of their journey, the people of <CityName> faced: ",
        "Time revealed its next trial when ",
        "Without warning, ",
        "The horizon darkened as ",
        "A pivotal moment arrived when ",
        "Destiny's hand stirred when ",
        "The course of history shifted as ",
        "A new chapter began when "
    };
    
    [Header("Outcome Introduction Prefixes")]
    [Tooltip("Used to introduce outcomes more dramatically")]
    [SerializeField] public List<string> outcomeIntros = new List<string>
    {
        "The consequences of this choice soon became clear: ",
        "As the dust settled, the result was undeniable: ",
        "Time would tell that ",
        "History would record that ",
        "The outcome proved decisive: ",
        "What followed changed everything: ",
        "The decision bore fruit when ",
        "In the wake of their action, ",
        "The path chosen led to an outcome none could ignore: ",
        "Fate's judgment came swiftly: "
    };
    
    [Header("Event Transition Phrases")]
    [Tooltip("Used between events within the same era")]
    [SerializeField] public List<string> eventTransitions = new List<string>
    {
        "But the trials were far from over...",
        "Yet another challenge awaited them...",
        "Scarcely had they recovered when...",
        "Time pressed onward, bringing new tests...",
        "The people of <CityName> had little time to rest before...",
        "As one chapter closed, another immediately opened..."
    };

    public string cityName;

    public string GetRandomOptionPrefix(OutcomeType outcome) 
    {
        if (positiveOptionPrefixes == null || positiveOptionPrefixes.Count == 0)
            return "";
        if (negativeOptionPrefixes == null || negativeOptionPrefixes.Count == 0)
            return "";


        switch(outcome)
        {
            case OutcomeType.Positive:
                return GetProcessedPrefix(positiveOptionPrefixes);
            case OutcomeType.Negative:
                return GetProcessedPrefix(negativeOptionPrefixes);
            default:
                return "";
        }

    }

    public string GetProcessedPrefix(List<string> prefixes)
    {
        int roll = UnityEngine.Random.Range(0, prefixes.Count);
        string processedPrefix = prefixes[roll];
        processedPrefix = processedPrefix.Replace("<CityName>", cityName);
        return processedPrefix;
    }

    public string GetRandomEventIntro()
    {
        if (eventIntros == null || eventIntros.Count == 0)
            return "";
            
        int roll = UnityEngine.Random.Range(0, eventIntros.Count);
        string processedIntro = eventIntros[roll];
        processedIntro = processedIntro.Replace("<CityName>", cityName);
        return processedIntro;
    }
    
    public string GetRandomOutcomeIntro()
    {
        if (outcomeIntros == null || outcomeIntros.Count == 0)
            return "";
            
        int roll = UnityEngine.Random.Range(0, outcomeIntros.Count);
        string processedIntro = outcomeIntros[roll];
        processedIntro = processedIntro.Replace("<CityName>", cityName);
        return processedIntro;
    }
    
    public string GetRandomEventTransition()
    {
        if (eventTransitions == null || eventTransitions.Count == 0)
            return "";
            
        int roll = UnityEngine.Random.Range(0, eventTransitions.Count);
        string processedTransition = eventTransitions[roll];
        processedTransition = processedTransition.Replace("<CityName>", cityName);
        return processedTransition;
    }
}
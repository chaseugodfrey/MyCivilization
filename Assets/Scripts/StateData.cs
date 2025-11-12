using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StateData", menuName = "Scriptable Objects/State")]
public class StateData : ScriptableObject
{
    int phase_index;
    int phase_count;

    [SerializeField] public List<string> phase1_texts;
    [SerializeField] public List<string> phase2_texts;
    [SerializeField] public List<string> phase3_texts;
    public List<List<string>> phases_texts => new List<List<string>>()
    {
        phase1_texts,
        phase2_texts,
        phase3_texts
    };

    public void InitState()
    {
        ResetState();
    }

    public bool AdvanceState()
    {
        phase_index++;

        if (phase_index >= phase_count)
        {
            return true;
        }

        return false;
    }

    public string RetrieveMessage()
    {
        int roll = Roll(phases_texts[phase_index].Count);
        Debug.Log("roll is " + roll);
        return phases_texts[phase_index][roll];
    }
    

    public void ResetState()
    {
        phase_count = phases_texts.Count;
        phase_index = 0;
    }

    int Roll(int max)
    {
        return UnityEngine.Random.Range(0, max);
    }
}

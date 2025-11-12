using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Era", menuName = "Scriptable Objects/Era")]
public class Era : ScriptableObject
{
    [SerializeField] string mEraName;
    [SerializeField] List<EraEvent> mEraEvents;

    public string GetEraName()
    {
        return mEraName;
    }

    public EraEvent GetRandomEvent()
    {
        // to do : reroll if event has been played
        int roll = Random.Range(0, mEraEvents.Count);
        return mEraEvents[roll];
    }
}

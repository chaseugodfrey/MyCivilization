using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Era", menuName = "Scriptable Objects/Era")]
public class Era : ScriptableObject
{
    [SerializeField] string mEraName;
    [SerializeField] string mEraDescription;
    [SerializeField] string mProcessedEraDescription;
    [SerializeField] List<EraEvent> mEraEvents;
    int mCurrentEventIndex = -1;

    public string GetEraName()
    {
        return mEraName;
    }

    public string GetEraDescription()
    {
        return mProcessedEraDescription;
    }

    public void SetEraDescription(string cityName)
    {
        mProcessedEraDescription = mEraDescription.Replace("<CityName>", cityName);
    }

    public List<EraEvent> GetEraEvents()
    {
        return mEraEvents;
    }

    public EraEvent GetRandomEvent()
    {
        // to do : reroll if event has been played
        int roll;
        if(mCurrentEventIndex == -1)
        {
            roll = Random.Range(0, mEraEvents.Count);
            mCurrentEventIndex = roll;
        }
        else
        {
            do
            {
                roll = UnityEngine.Random.Range(0, mEraEvents.Count);
            } while (roll == mCurrentEventIndex);
        }

        return mEraEvents[roll];
    }
}

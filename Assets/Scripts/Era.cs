using NUnit.Framework;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;

[CreateAssetMenu(fileName = "Era", menuName = "Scriptable Objects/Era")]
public class Era : ScriptableObject
{
    [SerializeField] string mEraName;
    [SerializeField] string mEraDescription;
    [SerializeField] string mProcessedEraDescription;
    [SerializeField] public List<EraEvent> mEraEvents;
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

    public EraEvent GetRandomEvent(int prosperity)
    {
        int roll;

        if (mCurrentEventIndex == -1)
        {
            roll = RandomManager.GetWeightedIndex(mEraEvents.Count, prosperity);
            mCurrentEventIndex = roll;
        }
        else
        {
            do
            {
                roll = RandomManager.GetWeightedIndex(mEraEvents.Count, prosperity);
            } while (roll == mCurrentEventIndex);

            mCurrentEventIndex = roll;
        }

        Debug.Log($"Selected event index: {roll} with prosperity: {prosperity}");
        return mEraEvents[roll];
    }
}

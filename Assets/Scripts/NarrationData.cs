using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

[CreateAssetMenu(fileName = "NarrationData", menuName = "Scriptable Objects/NarrationData")]
public class NarrationData : ScriptableObject
{
    [SerializeField] public List<string> optionPrefixes;
    public string cityName;

    public string GetRandomOptionPrefix()
    {
        int roll = UnityEngine.Random.Range(0, optionPrefixes.Count);
        string processedPrefix = optionPrefixes[roll];
        processedPrefix = processedPrefix.Replace("<CityName>", cityName);

        return processedPrefix;
    }
}

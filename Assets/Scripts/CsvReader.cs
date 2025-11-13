using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class CsvReader : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LoadNarrativeData();
    }

    public void LoadNarrativeData()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "narrativeData.csv");

        if (!File.Exists(filePath))
        {
            Debug.LogError("Cannot find the CSV file at: " + filePath);
            return;
        }

        string[] lines = File.ReadAllLines(filePath);
        NarrativeDatabase.Events.Clear();

        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i];
            string[] values = line.Split(',');

            if (values.Length < 8)
            {
                Debug.LogWarning("Skipping bad CSV line: " + line);
                continue;
            }

            EventData option = new EventData();
            try
            {
                option.EraID = int.Parse(values[0]);
                option.EventID = values[1];
                option.OptionID = values[2];
                option.OptionText = values[3].Replace("\"", "");

                option.PositiveOutcomeText = values[4].Replace("\"", "");
                option.PositivePS_Change = int.Parse(values[5]);

                option.NegativeOutcomeText = values[6].Replace("\"", "");
                option.NegativePS_Change = int.Parse(values[7]);

                if (!NarrativeDatabase.Events.ContainsKey(option.EventID))
                {
                    NarrativeDatabase.Events[option.EventID] = new List<EventData>();
                }

                NarrativeDatabase.Events[option.EventID].Add(option);
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error parsing line: " + line + "\n" + e.Message);
            }
        }

        Debug.Log("Loaded " + NarrativeDatabase.Events.Count + " events from CSV.");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

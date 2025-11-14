using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class CsvReader : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LoadNarrativeData();
    }

    public void LoadNarrativeData()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "GenericEraEvents.csv");

        Debug.Log("Trying to load CSV at: " + filePath);

        if (!File.Exists(filePath))
        {
            Debug.LogError("Cannot find the CSV file at: " + filePath);
            return;
        }

        Debug.Log("CSV file FOUND! Reading...");

        string[] lines = File.ReadAllLines(filePath);
        NarrativeDatabase.Events.Clear();

        Debug.Log("Total lines read (including header): " + lines.Length);

        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i];
            Debug.Log($"Processing line {i + 1}: {line}");
            List<string> values = ParseCSVLine(line);

            Debug.Log("Parsed Values (" + values.Count + "): " + string.Join(" | ", values));

            if (values.Count < 12)
            {
                Debug.LogWarning("Skipping bad CSV line: " + line);
                continue;
            }

            EventData option = new EventData();
            try
            {
                option.EraID = values[0];
                option.EraNarrative = values[1];
                option.EventID = values[2];
                option.EventNarrative = values[3];
                option.EventDifficulty = values[4];

                option.OptionID = values[5];
                option.OptionText = values[6];
                option.OptionTag = values[7];

                option.PositiveOutcomeText = values[8];
                option.PositivePS_Change = int.Parse(values[9]);
                option.NegativeOutcomeText = values[10];
                option.NegativePS_Change = int.Parse(values[11]);


                if (!NarrativeDatabase.Events.ContainsKey(option.EventID))
                {
                    NarrativeDatabase.Events[option.EventID] = new List<EventData>();
                }

                NarrativeDatabase.Events[option.EventID].Add(option);
                Debug.Log($"Added EventData -> EventID: {option.EventID}, OptionID: {option.OptionID}");
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error parsing line: " + line + "\n" + e.Message);
            }
        }

        Debug.Log("Loaded " + NarrativeDatabase.Events.Count + " events from CSV.");
    }

    private List<string> ParseCSVLine(string line)
    {
        List<string> values = new List<string>();
        StringBuilder currentValue = new StringBuilder();
        bool inQuotes = false;

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if (c == '"')
            {
                if (inQuotes && i < line.Length - 1 && line[i+1] == '"')
                {
                    currentValue.Append('"');
                    i++;
                }

                else
                {
                    inQuotes = !inQuotes;
                }
            }

            else if (c == ',')
            {
                if (inQuotes)
                {
                    currentValue.Append(c);
                }

                else
                {
                    values.Add(currentValue.ToString());
                    currentValue.Clear();
                }
            }

            else
            {
                currentValue.Append(c);
            }
        }

        values.Add(currentValue.ToString());

        return values;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

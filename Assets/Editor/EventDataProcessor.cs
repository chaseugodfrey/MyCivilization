// Place this script in a folder named "Editor" inside your Assets folder.
using UnityEngine;
using UnityEditor; // Required for editor scripts and asset creation
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq; // Used for GroupBy

public class EventDataProcessor
{
    // These paths MUST match your project's folder structure
    private const string ERAS_SAVE_PATH = "Assets/Eras";
    private const string EVENTS_SAVE_PATH = "Assets/Events";

    // This is the file to read from
    private const string CSV_FILE_NAME = "DetailedEraEvents.csv";

    // This function creates the new button in the Unity menu bar
    [MenuItem("Tools/Generate Game Events from CSV")]
    public static void GenerateScriptableObjects()
    {
        // 1. Load and Parse the CSV data
        // We re-use your CsvReader's logic, but store it locally
        Dictionary<string, List<EventData>> parsedEvents = LoadAndParseCSV();
        if (parsedEvents == null || parsedEvents.Count == 0)
        {
            Debug.LogError("CSV parsing failed or returned no data. Aborting.");
            return;
        }

        Debug.Log($"Successfully parsed {parsedEvents.Count} unique events from CSV.");

        // 2. Create the output folders if they don't exist
        if (!AssetDatabase.IsValidFolder(EVENTS_SAVE_PATH))
        {
            AssetDatabase.CreateFolder("Assets", "Events");
        }
        if (!AssetDatabase.IsValidFolder(ERAS_SAVE_PATH))
        {
            AssetDatabase.CreateFolder("Assets", "Eras");
        }

        // This will hold our newly created EraEvent SOs, ready to be grouped by Era
        Dictionary<string, List<EraEvent>> eraEventMap = new Dictionary<string, List<EraEvent>>();

        // 3. Process Each Event and Create EraEvent Scriptable Objects
        foreach (var eventPair in parsedEvents)
        {
            string uniqueEventKey = eventPair.Key; // e.g., "E1"
            List<EventData> options = eventPair.Value; // The 5 options for this event

            // Get event-wide data from the first option (it's the same for all)
            EventData firstOption = options[0];
            string eraID = firstOption.EraID; // e.g., "Stone-Bronze"

            // --- Create the new EraEvent Scriptable Object ---
            EraEvent newEventSO = ScriptableObject.CreateInstance<EraEvent>();

            // --- Map the CSV data to the EraEvent SO fields ---
            // We must use SerializedObject because your fields (mIntroMessage etc.) are private
            SerializedObject soEvent = new SerializedObject(newEventSO);
            soEvent.FindProperty("mIntroMessage").stringValue = firstOption.EraNarrative;
            soEvent.FindProperty("mActionMessage").stringValue = firstOption.EventNarrative; // You can change this if needed

            // Now we map the lists. Your EraEvent.GetOptionOutcome() logic expects
            // outcomes to be paired: [Pos, Neg, Pos, Neg, ...]
            List<string> optionMessages = new List<string>();
            List<string> outcomeMessages = new List<string>();
            List<int> outcomeValues = new List<int>();
            List<int> stableValues = new List<int>();

            foreach (EventData option in options)
            {
                optionMessages.Add(option.OptionText);

                // Add the Positive outcome, THEN the negative one
                outcomeMessages.Add(option.PositiveOutcomeText);
                outcomeMessages.Add(option.NegativeOutcomeText);

                // Add the Positive value, THEN the negative one
                outcomeValues.Add(option.PositivePS_Change);
                outcomeValues.Add(option.NegativePS_Change);

                stableValues.Add(option.PositiveStable_Change);
                stableValues.Add(option.NegativeStable_Change);
            }

            // --- Find the List<T> properties on the SO and set them ---
            soEvent.FindProperty("mOptionMessages").SetList(optionMessages);
            soEvent.FindProperty("mOutcomeMessages").SetList(outcomeMessages);
            soEvent.FindProperty("mOutcomeValues").SetList(outcomeValues);
            soEvent.FindProperty("mStableValues").SetList(stableValues);

            // Apply all changes to the SerializedObject
            soEvent.ApplyModifiedProperties();

            // --- Save the new .asset file ---
            string eventPath = $"{EVENTS_SAVE_PATH}/{uniqueEventKey}.asset";
            AssetDatabase.CreateAsset(newEventSO, eventPath);

            // 4. Add the newly created SO to our era map for the next step
            if (!eraEventMap.ContainsKey(eraID))
            {
                eraEventMap[eraID] = new List<EraEvent>();
            }
            eraEventMap[eraID].Add(newEventSO);

            Debug.Log($"Created asset: {eventPath}");
        }

        // 5. Process Each Era and Create Era Scriptable Objects
        foreach (var eraPair in eraEventMap)
        {
            string eraID = eraPair.Key; // "Stone-Bronze"
            List<EraEvent> eventsInEra = eraPair.Value; // The list of EraEvent SOs we just made

            // --- Create the new Era Scriptable Object ---
            Era newEraSO = ScriptableObject.CreateInstance<Era>();

            // Set the fields using SerializedObject
            SerializedObject soEra = new SerializedObject(newEraSO);
            soEra.FindProperty("mEraName").stringValue = eraID;
            soEra.FindProperty("mEraEvents").SetList(eventsInEra); // Assign the list of events
            soEra.ApplyModifiedProperties();

            // --- Save the new .asset file ---
            // We use eraID as the filename (e.g., "Stone-Bronze.asset")
            string eraPath = $"{ERAS_SAVE_PATH}/{eraID}.asset";
            AssetDatabase.CreateAsset(newEraSO, eraPath);
            Debug.Log($"Created asset: {eraPath}");
        }

        // 6. Save all changes to the Unity Asset Database
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("CSV Import Complete",
            $"Successfully generated {parsedEvents.Count} events and {eraEventMap.Count} eras.", "OK");

        GameObject eraManagerObj = GameObject.Find("EraManager");
        if (!eraManagerObj)
        {
            Debug.Log("Era Manager not found.");
            return;
        }

        EraManager eraManager = eraManagerObj.GetComponent<EraManager>();
        if (!eraManager)
        {
            Debug.Log("Era Manager script not added.");
            return;
        }

        bool success = eraManager.LoadEraAssets();
        if (!success)
        {
            Debug.Log("Folder not found.");
        }
        else
        {
            Debug.Log("Added.");
        }
    }

    [MenuItem("Tools/Delete All Game Events")]
    public static void ClearOldAssets()
    {
        Debug.Log("Cleaning old generated assets...");

        string[] eraGUIDS = AssetDatabase.FindAssets("t:Era", new[] { ERAS_SAVE_PATH });
        int eraCount = 0;

        foreach (string guid in eraGUIDS)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            AssetDatabase.DeleteAsset(path);
            eraCount++;
        }

        Debug.Log($"Deleted {eraCount} old Era assets from {ERAS_SAVE_PATH}");

        string[] eventGuids = AssetDatabase.FindAssets("t:EraEvent", new[] { EVENTS_SAVE_PATH });
        int eventCount = 0;

        foreach (string guid in eventGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            AssetDatabase.DeleteAsset(path);
            eventCount++;
        }

        Debug.Log($"Deleted {eventCount} old EraEvent assets from {EVENTS_SAVE_PATH}");

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("Clean Up Complete",
            $"Successfully deleted {eraCount} Eras and {eventCount} Events.", "OK");

        GameObject eraManagerObj = GameObject.Find("EraManager");
        if (!eraManagerObj)
        {
            Debug.Log("Era Manager not found.");
            return;
        }

        EraManager eraManager = eraManagerObj.GetComponent<EraManager>();
        if (!eraManager)
        {
            Debug.Log("Era Manager script not added.");
            return;
        }

        bool success = eraManager.LoadEraAssets();
        if (!success)
        {
            Debug.Log("Folder not found.");
        }
        else
        {
            Debug.Log("Cleared.");
        }
    }

    // This is your LoadNarrativeData function, modified to return the dictionary
    private static Dictionary<string, List<EventData>> LoadAndParseCSV()
    {
        string filePath = EditorUtility.OpenFilePanel("Assets", "Assets", "csv");
        //string filePath = Path.Combine(Application.streamingAssetsPath, CSV_FILE_NAME);
        var eventsDatabase = new Dictionary<string, List<EventData>>();

        if (!File.Exists(filePath))
        {
            Debug.LogError("Cannot find the CSV file at: " + filePath);
            return null;
        }

        ClearOldAssets();

        string[] lines = File.ReadAllLines(filePath);

        // Start at i = 1 to skip the header
        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i];
            List<string> values = ParseCSVLine(line);

            if (values.Count < 14)
            {
                Debug.LogWarning("Skipping bad CSV line: " + line);
                continue;
            }

            EventData option = new EventData();
            try
            {
                // This uses your EventData.cs class
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
                option.PositiveStable_Change = int.Parse(values[12]);
                option.NegativeStable_Change = int.Parse(values[13]);

                string uniqueEventKey = $"{option.EraID}-{option.EventID}";

                // Group all options by their EventID
                if (!eventsDatabase.ContainsKey(uniqueEventKey))
                {
                    eventsDatabase[uniqueEventKey] = new List<EventData>();
                }
                eventsDatabase[uniqueEventKey].Add(option);
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error parsing line: " + line + "\n" + e.Message);
            }
        }

        return eventsDatabase;
    }

    // This is your exact ParseCSVLine function, unchanged
    private static List<string> ParseCSVLine(string line)
    {
        List<string> values = new List<string>();
        StringBuilder currentValue = new StringBuilder();
        bool inQuotes = false;

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if (c == '"')
            {
                if (inQuotes && i < line.Length - 1 && line[i + 1] == '"')
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
}

// Helper extension methods to make setting List<T> properties on a SerializedObject easier
public static class SerializedPropertyExtensions
{
    // Sets a List<string>
    public static void SetList(this SerializedProperty prop, List<string> list)
    {
        prop.ClearArray();
        for (int i = 0; i < list.Count; i++)
        {
            prop.InsertArrayElementAtIndex(i);
            prop.GetArrayElementAtIndex(i).stringValue = list[i];
        }
    }

    // Sets a List<int>
    public static void SetList(this SerializedProperty prop, List<int> list)
    {
        prop.ClearArray();
        for (int i = 0; i < list.Count; i++)
        {
            prop.InsertArrayElementAtIndex(i);
            prop.GetArrayElementAtIndex(i).intValue = list[i];
        }
    }

    // Sets a List<ScriptableObject> (or any Unity Object)
    public static void SetList<T>(this SerializedProperty prop, List<T> list) where T : Object
    {
        prop.ClearArray();
        for (int i = 0; i < list.Count; i++)
        {
            prop.InsertArrayElementAtIndex(i);
            prop.GetArrayElementAtIndex(i).objectReferenceValue = list[i];
        }
    }
}
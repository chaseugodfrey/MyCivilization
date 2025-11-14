using UnityEngine;
using System.Collections.Generic;

public class EventData
{
    public string EraID { get; set; }
    public string EraNarrative { get; set; }
    public string EventID { get; set; }
    public string EventNarrative { get; set; }
    public string EventDifficulty { get; set; }
    public string OptionID { get; set; }
    public string OptionText { get; set; }
    public string OptionTag { get; set; }
    public string PositiveOutcomeText { get; set; }
    public int PositivePS_Change { get; set; }
    public string NegativeOutcomeText { get; set; }
    public int NegativePS_Change { get; set; }
    public int PositiveStable_Change { get; set; }
    public int NegativeStable_Change { get; set; }
}

public static class NarrativeDatabase
{
    public static Dictionary<string, List<EventData>> Events = new Dictionary<string, List<EventData>>();
}


using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class OutputManager : MonoBehaviour
{
    [Header("Customizable")]
    public string outputFileName;

    [SerializeField] List<string> outputTexts;

    public void AddOutputMessage(string msg)
    {
        outputTexts.Add(msg);
    }

    public void CreateOutputFile()
    {
        if (!outputFileName.Contains(".txt"))
        {
            outputFileName += ".txt";
        }

        File.WriteAllLines(outputFileName, outputTexts);
        outputTexts.Clear();
    }
}

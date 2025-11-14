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
        // Ensure .txt extension
        if (!outputFileName.EndsWith(".txt"))
            outputFileName += ".txt";

        // Step 1: project root folder (one level above Assets/)
        string projectRoot = Path.GetFullPath(
            Path.Combine(Application.dataPath, "..")
        );

        // Step 2: Output folder inside root
        string outputFolder = Path.Combine(projectRoot, "Output");

        // Create folder if missing
        if (!Directory.Exists(outputFolder))
            Directory.CreateDirectory(outputFolder);

        // Step 3: Full file path
        string filePath = Path.Combine(outputFolder, outputFileName);

        // Step 4: Make unique filename if needed
        filePath = GetUniqueFilePath(filePath);

        // Step 5: Write file
        File.WriteAllLines(filePath, outputTexts);

        outputTexts.Clear();
        Debug.Log("Output saved to: " + filePath);
    }

    private string GetUniqueFilePath(string filePath)
    {
        if (!File.Exists(filePath))
            return filePath;

        string dir = Path.GetDirectoryName(filePath);
        string name = Path.GetFileNameWithoutExtension(filePath);
        string ext = Path.GetExtension(filePath);

        int i = 1;
        string newPath;

        do
        {
            newPath = Path.Combine(dir, $"{name} ({i}){ext}");
            i++;
        }
        while (File.Exists(newPath));

        return newPath;
    }
}

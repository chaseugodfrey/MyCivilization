using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OutputManager : MonoBehaviour
{
    [Header("Customizable")]
    public string outputFileName;
    public TextMeshProUGUI text_finalOutput;
    public string cityName;

    [SerializeField] List<string> outputTexts;

    private IEnumerator RefreshScroll()
    {
        // Wait one frame for TMP to update text mesh
        yield return null;

        // Force rebuild of vertical layout / content size
        LayoutRebuilder.ForceRebuildLayoutImmediate(
            text_finalOutput.GetComponent<RectTransform>()
        );

        // OPTIONAL: Scroll to top or bottom after update
        ScrollRect scroll = text_finalOutput.GetComponentInParent<ScrollRect>();
        if (scroll)
            scroll.normalizedPosition = new Vector2(0, 1); // scroll to top
    }

    public void UpdateFinalOutputTextUI()
    {
        if (outputTexts != null && outputTexts.Count > 0)
            text_finalOutput.text = string.Join("\n",outputTexts);
        else
            text_finalOutput.text = "";

        StartCoroutine(RefreshScroll());
    }

    public void AddOutputMessage(string msg)
    {
        outputTexts.Add(msg);
    }

    void ModifyOutputFileName()
    {
        outputFileName = $"The History of {cityName}.txt";
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

        //Step 2.5 Create File Name with City
        ModifyOutputFileName();

        // Step 3: Full file path
        string filePath = Path.Combine(outputFolder, outputFileName);

        // Step 4: Make unique filename if needed
        filePath = GetUniqueFilePath(filePath);

        // Step 5: Write file
        File.WriteAllLines(filePath, outputTexts);

        outputTexts.Clear();
        Debug.Log("Output saved to: " + filePath);

        OpenOutputFileInExplorer(filePath);
    }

    public void OpenOutputFileInExplorer(string filePath)
    {
#if UNITY_EDITOR
        // Highlight the file in Unity Editor
        UnityEditor.EditorUtility.RevealInFinder(filePath);
#else
    // Open in Windows File Explorer
    if (Application.platform == RuntimePlatform.WindowsPlayer ||
        Application.platform == RuntimePlatform.WindowsEditor)
    {
        // If you want highlight: explorer.exe /select,"path"
        System.Diagnostics.Process.Start("explorer.exe", "/select,\"" + filePath + "\"");
    }
    // Open in macOS Finder
    else if (Application.platform == RuntimePlatform.OSXPlayer ||
             Application.platform == RuntimePlatform.OSXEditor)
    {
        System.Diagnostics.Process.Start("open", "-R \"" + filePath + "\"");
    }
    // Linux—just open the folder
    else if (Application.platform == RuntimePlatform.LinuxPlayer)
    {
        string directory = Path.GetDirectoryName(filePath);
        System.Diagnostics.Process.Start("xdg-open", directory);
    }
#endif
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

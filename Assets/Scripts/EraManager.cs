using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class EraManager : MonoBehaviour
{
    public string folderName;

    public List<Era> eraObjs;
    private int index = -1;

    private void Awake()
    {

    }

    public void ClearEraAssets()
    {
        eraObjs.Clear();
    }

    public bool LoadEraAssets()
    {
        eraObjs = new List<Era>();
        string folderPath = Path.Combine("Assets\\", folderName);

        if (!Directory.Exists(folderPath))
        {
            return false;
        }

        string[] assetFilePaths = Directory.GetFiles(folderPath);
        foreach (var filepath in assetFilePaths)
        {
            if (filepath.EndsWith(".meta"))
                continue;
            Era era = AssetDatabase.LoadAssetAtPath<Era>(filepath);
            eraObjs.Add(era);
        }

        return true;
    }

    public Era GetNextEraObj()
    {
        ++index;
        return GetEraObj(index);
    }

    public Era GetEraObj(int index)
    {
        if (eraObjs.Count == 0)
            return ScriptableObject.CreateInstance<Era>();

        if (index > eraObjs.Count)
            return eraObjs[0];

        return eraObjs[index];
    }
}

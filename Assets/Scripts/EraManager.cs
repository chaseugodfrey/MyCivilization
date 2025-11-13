using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class EraManager : MonoBehaviour
{
    public List<Era> eraObjs;

    public Era GetRandomEraObj()
    {
        int roll = Random.Range(0, eraObjs.Count);
        return GetEraObj(roll);
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

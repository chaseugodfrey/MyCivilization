using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class EraManager : MonoBehaviour
{
    public List<Era> eraObjs;
    private int index = -1;

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

using UnityEngine;
using System.Collections.Generic;

public class RandomManager : MonoBehaviour
{
    public static int GetWeightedIndex(int count, int prosperity)
    {
        if (count <= 0) return -1;
        if (count == 1) return 0;

        // Normalize prosperity to 0-1 range
        float normalizedProsperity = Mathf.Clamp01(prosperity / 100f);

        float[] weights = new float[count];
        float totalWeight = 0f;

        for (int i = 0; i < count; i++)
        {
            float prosperityFactor = Mathf.Lerp(0.5f, 1.0f, normalizedProsperity);
            weights[i] = Mathf.Pow(prosperityFactor, i);
            totalWeight += weights[i];
        }

        float randomValue = UnityEngine.Random.Range(0f, totalWeight);
        float cumulative = 0f;

        for (int i = 0; i < count; i++)
        {
            cumulative += weights[i];
            if (randomValue <= cumulative)
            {
                return i;
            }
        }

        return count - 1;
    }
}
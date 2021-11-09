using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CustomFunctions
{
    public static float Remap(this float value, float FromMin, float FromMax, float ToMin, float ToMax)
    {
        return (value - FromMin) / (FromMax - FromMin) * (ToMax - ToMin) + ToMin;
    }

    public static int[] ShuffleIntArray(int[] Input)
    {
        List<int> input = new List<int>(Input);
        int[] array = new int[Input.Length];
        int rng = Random.Range(0, input.Count);

        for (int i = 0; i < array.Length; i++)
        {
            array[i] = input[rng];
            input.RemoveAt(rng);
            rng = Random.Range(0, input.Count);
        }

        return array;
    }
}

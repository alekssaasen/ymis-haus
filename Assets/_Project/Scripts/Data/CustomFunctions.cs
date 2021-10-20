using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CustomFunctions
{
    public static float Remap(this float value, float FromMin, float FromMax, float ToMin, float ToMax)
    {
        return (value - FromMin) / (FromMax - FromMin) * (ToMax - ToMin) + ToMin;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    public static Button button;

    public void Start()
    {
        button = this;
    }
}

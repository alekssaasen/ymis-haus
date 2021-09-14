using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    void Update()
    {
        // ------------ Decent placeholder code ------------------------------------------------------------
        if (GameManager.Main.localPlayerID == 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (GameManager.Main.localPlayerID == 1)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
    }
}

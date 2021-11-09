using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUI_SkipTurnBar : MonoBehaviour
{
    public RectTransform slider;
    public float turnTime;

    private bool countDown;
    [Min(0.1f)] private float time = 60;

    public void StartCount()
    {
        time = 0;
        slider.localScale = new Vector3(0, 1, 1);
        countDown = true;
    }

    private void Update()
    {
        if (countDown)
        {
            time += Time.deltaTime;

            if (time >= turnTime)
            {
                countDown = false;
                GameLoop.Main.FinishLocalTurn();
            }
            slider.localScale = new Vector3(time / turnTime, 1, 1);
        }
    }
}

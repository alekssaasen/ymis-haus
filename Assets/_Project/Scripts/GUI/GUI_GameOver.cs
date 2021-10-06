using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GUI_GameOver : MonoBehaviour
{
    public TMP_Text text;
    public GameObject background;

    public void EndGame(bool LocalPlayerWins)
    {
        if (LocalPlayerWins)
        {
            background.SetActive(true);
            text.text = "You win";
            text.color = new Color(0, 1, 0);
        }
        else
        {
            background.SetActive(true);
            text.text = "You lose";
            text.color = new Color(1, 0, 0);
        }
    }
}

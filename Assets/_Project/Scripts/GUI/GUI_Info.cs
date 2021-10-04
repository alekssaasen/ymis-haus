using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GUI_Info : MonoBehaviour
{
    public TMP_Text turnCountText;
    public TMP_Text goldCountText;

    private void FixedUpdate()
    {
        if (GameManager.Main.turnID == GameManager.Main.localPlayerID)
        {
            goldCountText.text = EconomySystem.Money + "$";
            turnCountText.text = GameManager.Main.turnPointsLeft + "¤";
        }
        else
        {
            goldCountText.text = "*$";
            turnCountText.text = "*¤";
        }
    }
}

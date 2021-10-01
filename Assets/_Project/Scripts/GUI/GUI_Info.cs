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
        goldCountText.text = EconomySystem.Money + "$";
        turnCountText.text = GameManager.Main.turnPointsLeft + "¤";
    }
}

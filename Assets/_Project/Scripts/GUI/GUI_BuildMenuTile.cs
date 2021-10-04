using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class GUI_BuildMenuTile : MonoBehaviour
{
    public bool canBuy;

    public TMP_Text goldText;
    public Image errorImage;
    public TMP_Text errorText;
    public RawImage backgroundImage;

    public ChessFigure figure;
    public ChessBuiding buiding;

    private void FixedUpdate()
    {
        if (EconomySystem.CanBuyFigure(figure, out string msg1) || EconomySystem.CanBuyBuilding(buiding, out string msg2))
        {
            errorText.transform.parent.gameObject.SetActive(false);
            errorText.text = "";
            canBuy = true;
        }
        else
        {
            errorText.transform.parent.gameObject.SetActive(true);
            errorText.text = msg1 + msg2;
            canBuy = false;
        }

        if (figure != ChessFigure.Empty && EconomySystem.CheckFigurePrice(figure, out int num1))
        {
            goldText.text = num1.ToString();
        }

        if (buiding != ChessBuiding.Empty && EconomySystem.CheckBuildingPrice(buiding, out int num2))
        {
            goldText.text = num2.ToString();
        }
    }

    public void Click()
    {
        if (!canBuy)
        {
            GUI_MainMessage.SendNewMessage(errorText.text);
        }
    }
}

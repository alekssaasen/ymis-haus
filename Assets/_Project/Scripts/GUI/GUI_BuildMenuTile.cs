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
    public ChessBuiding building;

    private void FixedUpdate()
    {
        if (figure != ChessFigure.Empty && building == ChessBuiding.Empty)
        {
            bool canbuyfigure = EconomySystem.CanBuyFigure(figure, out string msg, out int price);
            if (canbuyfigure)
            {
                errorText.transform.parent.gameObject.SetActive(false);
                errorText.text = "";
                canBuy = true;
                goldText.text = price.ToString();
            }
            else
            {
                errorText.transform.parent.gameObject.SetActive(true);
                errorText.text = msg;
                canBuy = false;
                goldText.text = price.ToString();
            }
        }
        else if (building != ChessBuiding.Empty && figure == ChessFigure.Empty)
        {
            bool canbuybuilding = EconomySystem.CanBuyBuilding(building, out string msg, out int price);
            if (canbuybuilding)
            {
                errorText.transform.parent.gameObject.SetActive(false);
                errorText.text = "";
                canBuy = true;
                goldText.text = price.ToString();
            }
            else
            {
                errorText.transform.parent.gameObject.SetActive(true);
                errorText.text = msg;
                canBuy = false;
                goldText.text = price.ToString();
            }
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

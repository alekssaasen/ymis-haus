using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;// Required when using Event data.

public class GUI_Select : MonoBehaviour, ISelectHandler //This Interface is required to receive OnDeselect callbacks.
{
    public GUI_BuildMenuTile tile;

    public void OnSelect(BaseEventData data)
    {
        if (tile.canBuy)
        {
            Debug.Log("Selected: " + gameObject.name);
            GameLoop.Main.ChangeState(tile.figure, tile.buiding);
        }
        else
        {
            GameLoop.Main.figureSelected = ChessFigure.Empty;
            GameLoop.Main.buildingSelected = ChessBuiding.Empty;
            GameLoop.Main.ChangeState(ChessFigure.Empty, ChessBuiding.Empty);
        }
    }
}
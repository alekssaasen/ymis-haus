using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GUI_InfoBox : MonoBehaviour
{
    private RectTransform rT;
    private bool open = true;

    [SerializeField] private TMP_Text title, description;
    [SerializeField] private Image image;
    [SerializeField]private InfoData[] infoDatas;

    private void Start()
    {
        rT = GetComponent<RectTransform>();
        SetData(InfoType.MineInfo);
    }

    public void SwitchState()
    {
        open = !open;
        if (open)
        {
            rT.anchoredPosition = new Vector2(85, 0);
        }
        else
        {
            rT.anchoredPosition = new Vector2(-85, 0);
        }
    }

    public void ChangeInfo(TileInfo tileInfo)
    {
        if (tileInfo.building != ChessBuiding.Empty)
        {
            switch (tileInfo.building)
            {
                case ChessBuiding.Farm:
                    SetData(InfoType.FarmInfo);
                    break;
                case ChessBuiding.Mine:
                    SetData(InfoType.MineInfo);
                    break;
                case ChessBuiding.Barracks:
                    SetData(InfoType.BarrackInfo);
                    break;
                default:
                    SetData(InfoType.EmptyInfo);
                    break;
            }
        }
        else if (tileInfo.figure != ChessFigure.Empty)
        {
            switch (tileInfo.figure)
            {
                case ChessFigure.King:
                    SetData(InfoType.KingInfo);
                    break;
                case ChessFigure.Queen:
                    SetData(InfoType.QueenInfo);
                    break;
                case ChessFigure.Bishop:
                    SetData(InfoType.BishopInfo);
                    break;
                case ChessFigure.Knight:
                    SetData(InfoType.KnightInfo);
                    break;
                case ChessFigure.Rook:
                    SetData(InfoType.RookInfo);
                    break;
                case ChessFigure.Pawn:
                    SetData(InfoType.PawnInfo);
                    break;
                default:
                    SetData(InfoType.EmptyInfo);
                    break;
            }
        }
        else
        {
            SetData(InfoType.EmptyInfo);
        }
    }

    private void SetData(InfoType infoType)
    {
        InfoBoxData data = new InfoBoxData();
        foreach (InfoData infoData in infoDatas)
        {
            if (infoData.infoType == infoType)
            {
                data = infoData.infoBoxData;
            }
        }
        title.text = data.title;
        description.text = data.description;
        image.sprite = data.imageSprite;
    }

}


public enum InfoType
{
    EmptyInfo,MineInfo,FarmInfo,BarrackInfo,KingInfo,QueenInfo,BishopInfo,KnightInfo,RookInfo,PawnInfo
}

[System.Serializable]
public struct InfoData
{
    public InfoBoxData infoBoxData;
    public InfoType infoType;
}
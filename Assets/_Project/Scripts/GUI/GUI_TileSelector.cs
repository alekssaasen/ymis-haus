using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GUI_TileSelector : MonoBehaviour
{
    private Ray ray;
    private RaycastHit hit;
    public Transform mouseMarker;
    public GUI_InfoBox infoBox;
    public void ClickOnTile(bool rightclicked)
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray.origin, ray.direction, out hit))
        {
            Vector2Int pos = new Vector2Int(Mathf.RoundToInt(hit.point.x), Mathf.RoundToInt(hit.point.z));
            Debug.LogFormat("Pos: {0}, OwnerID: {1}, DefaultID: {2}", pos, GameManager.Main.Board[pos.x, pos.y].ownerID, GameManager.Main.Board[pos.x, pos.y].defaultID);
            GameLoop.Main.NewPositionSelected(pos);
            if (FigureMovement.InsideBoard(pos) && rightclicked)
            {
                infoBox.ChangeInfo(GameManager.Main.Board[pos.x, pos.y]);
            }
        }
        else
        {
            Debug.Log(-Vector2Int.one);
            GameLoop.Main.NewPositionSelected(-Vector2Int.one);
        }
    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            ClickOnTile(true);
        }

        mouseMarker.localPosition = new Vector3(0, 0, 1);
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray.origin, ray.direction, out hit))
        {
            Vector2Int pos = new Vector2Int(Mathf.RoundToInt(hit.point.x), Mathf.RoundToInt(hit.point.z));
            if (GameLoop.Main.validNewFigurePositions.Contains(pos))
            {
                mouseMarker.localPosition = new Vector3(pos.x + 0.5f, pos.y + 0.5f, 0);
            }
        }
    }
}

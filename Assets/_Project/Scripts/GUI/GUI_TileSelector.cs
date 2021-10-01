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
    
    public void ClickOnTile()
    {
        mouseMarker.localPosition = new Vector3(0, 0, 1);

        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray.origin, ray.direction, out hit))
        {
            Vector2Int pos = new Vector2Int(Mathf.RoundToInt(hit.point.x), Mathf.RoundToInt(hit.point.z));
            Debug.Log("Pos: " + pos + ", ID: " + GameManager.Main.Board[pos.x, pos.y].ownerID);
            GameLoop.Main.NewPositionSelected(pos);
        }
        else
        {
            Debug.Log(-Vector2Int.one);
            GameLoop.Main.NewPositionSelected(-Vector2Int.one);
        }
        
    }

    public void Update()
    {
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

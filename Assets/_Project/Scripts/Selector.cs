using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selector : MonoBehaviour
{
    private Vector2Int SelectedPosition;
    private Ray ray;
    private RaycastHit hit;
    public Transform mouseMarker;

    void Update()
    {
        // ------------ Shit placeholder code --------------------------------------------------------------
        mouseMarker.localPosition = new Vector3(0, 0, 1);
        if (Input.GetMouseButtonDown(0))
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray.origin, ray.direction, out hit))
            {
                Vector2Int pos = new Vector2Int(Mathf.RoundToInt(hit.point.x), Mathf.RoundToInt(hit.point.z));
                Debug.Log("Pos: " + pos + ", ID: " + GameManager.Main.Board[pos.x,pos.y].ownerID);
                GameLoop.Main.NewPositionSelected(pos);
                SelectedPosition = pos;
            }
            else
            {
                Debug.Log(-Vector2Int.one);
                GameLoop.Main.NewPositionSelected(-Vector2Int.one);
                SelectedPosition = -Vector2Int.one;
            }
        }
        else
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

    private void OnDrawGizmos()
    {
        Gizmos.DrawCube(new Vector3(SelectedPosition.x, 0, SelectedPosition.y), new Vector3(1, 0.07f, 1));
        Gizmos.DrawLine(transform.position, transform.position + ray.direction * 25);
        Gizmos.DrawSphere(hit.point, 0.1f);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selector : MonoBehaviour
{
    public static Vector2Int SelectedPosition;
    private Ray ray;
    private RaycastHit hit;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Physics.Raycast(ray.origin, ray.direction, out hit);

            Vector3 hitpoint = hit.point + new Vector3(0.5f, 0, 0.5f);

            Vector2Int pos = new Vector2Int((int)hitpoint.x, (int)hitpoint.z);

            if (pos.x >= 0 && pos.x < 8 && pos.y >= 0 && pos.y < 8)
            {
                GameManager.Main.PositionSelected(Vector2Int.RoundToInt(pos));
                SelectedPosition = Vector2Int.RoundToInt(pos);
            }
            else
            {
                GameManager.Main.PositionSelected(-Vector2Int.one);
                SelectedPosition = -Vector2Int.one;
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

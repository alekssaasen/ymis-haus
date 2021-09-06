using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selector : MonoBehaviour
{
    public static Vector2Int SelectedPosition;
    private Ray ray;

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Physics.Raycast(ray.origin, ray.direction, out RaycastHit hit);

            Vector2Int pos = new Vector2Int((int)hit.point.x, (int)hit.point.z);

            if (pos.x >= 0 && pos.x < 8 && pos.y >= 0 && pos.y < 8)
            {
                SelectedPosition = pos;
            }
            else
            {
                SelectedPosition = Vector2Int.zero;
                ray = new Ray();
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawCube(new Vector3(SelectedPosition.x, 0.125f, SelectedPosition.y), new Vector3(1, 0.25f, 1));
    }
}

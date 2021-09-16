using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Vector2 minMove;
    public Vector2 maxMove;
    public float movementSpeed;

    private Vector3 move;
    private Vector3 newmove;

    void Update()
    {
        // ------------ Decent placeholder code ------------------------------------------------------------
        if (GameManager.Main.localPlayerID == 0)
        {
            transform.rotation = Quaternion.Euler(0, 45, 0);
        }
        else if (GameManager.Main.localPlayerID == 1)
        {
            transform.rotation = Quaternion.Euler(0, 225, 0);
        }

        move = (transform.right * Input.GetAxisRaw("Horizontal") + transform.forward * Input.GetAxisRaw("Vertical")).normalized * Time.deltaTime * movementSpeed;
        newmove = transform.position + move;
        transform.position = new Vector3(Mathf.Min(maxMove.x, Mathf.Max(minMove.x, newmove.x)), 0, Mathf.Min(maxMove.y, Mathf.Max(minMove.y, newmove.z)));
    }
}

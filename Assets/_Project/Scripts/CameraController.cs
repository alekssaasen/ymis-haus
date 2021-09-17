using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Vector2 minMove = new Vector2(1, 1);
    private Vector2 maxMove;
    public float movementSpeed;

    private Vector3 move;
    private Vector3 newmove;

    private void Start()
    {
        minMove = new Vector2(1, 1);
        maxMove = new Vector2(GameManager.Main.Board.GetLength(0) - 2, GameManager.Main.Board.GetLength(1) - 2);
    }

    private void Update()
    {
        float offset = 0;
        if (Input.GetKey(KeyCode.Q))
        {
            offset += 40;
        }
        if (Input.GetKey(KeyCode.E))
        {
            offset -= 40;
        }

        // ------------ Decent placeholder code ------------------------------------------------------------
        if (GameManager.Main.ChessGameSettings.ChessGameMode == ChessGameModes.ChessEmpires)
        {
            if (GameManager.Main.localPlayerID == 0)
            {
                transform.rotation = Quaternion.Euler(0, 45 + offset, 0);
            }
            else if (GameManager.Main.localPlayerID == 1)
            {
                transform.rotation = Quaternion.Euler(0, 225 + offset, 0);
            }
        }
        else
        {
            if (GameManager.Main.localPlayerID == 0)
            {
                transform.rotation = Quaternion.Euler(0, offset, 0);
            }
            else if (GameManager.Main.localPlayerID == 1)
            {
                transform.rotation = Quaternion.Euler(0, 180 + offset, 0);
            }
        }

        move = (transform.right * Input.GetAxisRaw("Horizontal") + transform.forward * Input.GetAxisRaw("Vertical")).normalized * Time.deltaTime * movementSpeed;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            move *= 3;
        }
        newmove = transform.position + move;
        transform.position = new Vector3(Mathf.Min(maxMove.x, Mathf.Max(minMove.x, newmove.x)), 0, Mathf.Min(maxMove.y, Mathf.Max(minMove.y, newmove.z)));
    }
}

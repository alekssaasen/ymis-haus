using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Vector2 minMove = new Vector2(1, 1);
    private Vector2 maxMove;
    private Vector2 minMaxZoom = new Vector2(-3, 5);
    public float movementSpeed;

    private Vector3 move;
    private Vector3 newmove;

    public void Initialize()
    {
        minMove = new Vector2(1, 1);
        maxMove = new Vector2(GameManager.Main.Board.GetLength(0) - 2, GameManager.Main.Board.GetLength(1) - 2);
        transform.position = new Vector3(6.5f, 0, 6.5f);
        transform.position = GameManager.GameSettingsInUse.CameraPositionOffsets[GameManager.Main.localPlayerID];
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

        transform.rotation = Quaternion.Euler(0, GameManager.GameSettingsInUse.CameraRotationOffsets[GameManager.Main.localPlayerID] + offset, 0);

        move = (transform.right * Input.GetAxisRaw("Horizontal") + transform.forward * Input.GetAxisRaw("Vertical")).normalized * Time.deltaTime * movementSpeed;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            move *= 3;
        }
        newmove = transform.position + move;
        transform.position = new Vector3(Mathf.Min(maxMove.x, Mathf.Max(minMove.x, newmove.x)), 0, Mathf.Min(maxMove.y, Mathf.Max(minMove.y, newmove.z)));
    }
}

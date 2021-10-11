using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Vector2 minMove;
    private Vector2 maxMove;
    private Vector3 cameraPosition;
    private float zoom = 7;
    public Camera cameraRig;
    public float movementSpeed;
    public Vector2 minMaxZoom = new Vector2(1, 10);

    private Vector3 move;
    private Vector3 newmove;

    public void Initialize()
    {
        cameraPosition = cameraRig.transform.localPosition.normalized;
        cameraPosition = new Vector3(0, 6, -4).normalized;

        minMove = new Vector2(1, 1);
        maxMove = new Vector2(GameManager.Main.Board.GetLength(0) - 2, GameManager.Main.Board.GetLength(1) - 2);
        transform.position = new Vector3(6.5f, 0, 6.5f);
        transform.position = GameManager.GameSettingsInUse.CameraPositionOffsets[GameManager.Main.localPlayerID];
    }

    private void Update()
    {
        transform.rotation = Quaternion.Euler(0, GameManager.GameSettingsInUse.CameraRotationOffsets[GameManager.Main.localPlayerID] + 40 * Input.GetAxis("RotationX"), 0);

        move = (transform.right * Input.GetAxis("Horizontal") + transform.forward * Input.GetAxis("Vertical")).normalized * Time.deltaTime * movementSpeed;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            move *= 3;
        }
        newmove = transform.position + move;
        transform.position = new Vector3(Mathf.Min(maxMove.x, Mathf.Max(minMove.x, newmove.x)), 0, Mathf.Min(maxMove.y, Mathf.Max(minMove.y, newmove.z)));

        zoom += Input.GetAxis("ZoomY") * Time.deltaTime * 10;
        zoom = Mathf.Clamp(zoom, minMaxZoom.x, minMaxZoom.y);
        cameraRig.transform.localPosition = cameraPosition * zoom;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClasicBoard : MonoBehaviour
{
    public Transform board;

    public Transform[] Walls;

    void OnEnable()
    {
        board.localScale = new Vector3(GameManager.GameSettingsInUse.BoardSetup.MapSize.x,
            (GameManager.GameSettingsInUse.BoardSetup.MapSize.x + GameManager.GameSettingsInUse.BoardSetup.MapSize.y) / 2f,
            GameManager.GameSettingsInUse.BoardSetup.MapSize.y);
        board.position = board.localScale * 0.5f;
        board.position = Vector3.Scale(transform.position, new Vector3(1, 0, 1));
        board.position = new Vector3((GameManager.Main.Board.GetLength(0) - 1) * 0.5f, 0, (GameManager.Main.Board.GetLength(1) - 1) * 0.5f);

        board.GetComponent<MeshRenderer>().materials[1].mainTextureScale = (Vector2)GameManager.GameSettingsInUse.BoardSetup.MapSize * 0.5f;

        if (!GameManager.GameSettingsInUse.ClassicMovement)
        {
            Walls[0].position = new Vector3(0, 0, 0);
            Walls[1].position = new Vector3(GameManager.Main.Board.GetLength(0), 0, GameManager.Main.Board.GetLength(1));
            Walls[2].position = new Vector3(0, 0, GameManager.Main.Board.GetLength(1));
            Walls[3].position = new Vector3(GameManager.Main.Board.GetLength(0), 0, 0);

            for (int i = 0; i < Photon.Pun.PhotonNetwork.PlayerList.Length; i++)
            {
                Walls[i].gameObject.SetActive(true);
            }
        }
    }
}

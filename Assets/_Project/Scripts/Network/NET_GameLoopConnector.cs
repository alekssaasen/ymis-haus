using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class NET_GameLoopConnector : MonoBehaviourPunCallbacks
{
    [PunRPC]
    public void MoveFigure(Vector2 OldPosition, Vector2 NewPosition)
    {
        Debug.Log("Moving figure to: " + NewPosition.ToString());
        GameManager.Main.MovePiece(Vector2Int.RoundToInt(OldPosition), Vector2Int.RoundToInt(NewPosition));
    }

    [PunRPC]
    public void PlaceBuilding(Vector2 NewPosition, ChessBuiding Building, int NewID)
    {
        Debug.Log("Building: " + Building.ToString());
        GameManager.Main.BuildBuilding(Vector2Int.RoundToInt(NewPosition), Building, NewID);
    }

    [PunRPC]
    public void SpawnFigure(Vector2 NewPosition, ChessFigure Figure, int NewID)
    {
        Debug.Log("Spawning: " + Figure.ToString());
        GameManager.Main.SpawnFigure(Vector2Int.RoundToInt(NewPosition), Figure, NewID);
    }

    [PunRPC]
    public void FinishTurn()
    {
        GameManager.Main.turnID += 1;
        if (GameManager.Main.turnID == PhotonNetwork.PlayerList.Length)
        {
            GameManager.Main.turnID = 0;
        }

        if (GameManager.Main.localPlayerID == GameManager.Main.turnID)
        {
            GUI_MainMessage.SendNewMessage("Your turn!");
        }
        else
        {
            GUI_MainMessage.SendNewMessage(NET_GameLoopSetup.GetPlayerByID(GameManager.Main.turnID).NickName + "'s turn!");
        }

        GameLoop.Main.StartLocalTurn();
    }

    [PunRPC]
    public void DestroyPlayer(int ID)
    {
        for (int x = 0; x < GameManager.Main.Board.GetLength(0); x++)
        {
            for (int y = 0; y < GameManager.Main.Board.GetLength(1); y++)
            {
                if (GameManager.Main.Board[x, y].ownerID == ID)
                {
                    GameManager.Main.Board[x, y].ownerID = GameManager.Main.Board[x, y].defaultID;
                    GameManager.Main.Board[x, y].figure = ChessFigure.Empty;
                    GameManager.Main.Board[x, y].figureTransform = null;
                    GameManager.Main.Board[x, y].building = ChessBuiding.Empty;
                    GameManager.Main.Board[x, y].buildingTransform = null;
                }
            }
        }

        GameManager.Main.UpdateFigures();
    }

    public void LeaveRoom()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom(false);
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom(false);
        }
    }
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(0);
    }
}

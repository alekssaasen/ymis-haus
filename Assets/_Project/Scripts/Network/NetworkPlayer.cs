using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using TMPro;
using Photon.Realtime;

public class NetworkPlayer : MonoBehaviourPunCallbacks
{
    public TMP_Text textID;
    public string[] playerNamesByIndex;

    private void Awake()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("Choosing sides");
            int[] players = new int[PhotonNetwork.PlayerList.Length];
            List<int> ids = new List<int>();

            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                ids.Add(i);
            }

            for (int i = 0; i < players.Length; i++)
            {
                int random = Random.Range(0, ids.Count);
                players[i] = ids[random];
                ids.Remove(ids[random]);
            }

            PhotonView.Get(this).RpcSecure("SetSide", RpcTarget.AllBufferedViaServer, false, players);
        }
    }

    [PunRPC]
    public void SetSide(int[] PlayerID)
    {
        playerNamesByIndex = new string[PlayerID.Length];
        Debug.Log("Applying side to local");
        for (int i = 0; i < PlayerID.Length; i++)
        {
            playerNamesByIndex[PlayerID[i]] = PhotonNetwork.PlayerList[i].NickName;
            if (PhotonNetwork.LocalPlayer.ActorNumber == PhotonNetwork.PlayerList[i].ActorNumber)
            {
                GameManager.Main.localPlayerID = PlayerID[i];
                textID.text = "ID: " + GameManager.Main.localPlayerID;
            }
        }
        GameLoop.Main.cameraController.Initialize();
    }

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

        if (PhotonNetwork.LocalPlayer.NickName == playerNamesByIndex[GameManager.Main.turnID])
        {
            GUI_MainMessage.SendNewMessage("Your turn!");
        }
        else
        {
            GUI_MainMessage.SendNewMessage(playerNamesByIndex[GameManager.Main.turnID] + "'s turn!");
        }

        if (GameManager.Main.turnID == GameManager.Main.localPlayerID)
        {
            GameLoop.Main.StartLocalTurn();
            GameLoop.Main.NewPositionSelected(-Vector2Int.one);
        }
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

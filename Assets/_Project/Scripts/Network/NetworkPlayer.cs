using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using TMPro;
using Photon.Realtime;

public class NetworkPlayer : MonoBehaviourPunCallbacks
{
    public TMP_Text text;
    public int ID;

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
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
        for (int i = 0; i < PlayerID.Length; i++)
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber == PhotonNetwork.PlayerList[i].ActorNumber)
            {
                GameManager.Main.localPlayerID = PlayerID[i];
                text.text = "ID: " + GameManager.Main.localPlayerID;
            }
        }
    }

    [PunRPC]
    public void FinishTurn(Vector2 OldPosition, Vector2 NewPosition)
    {
        GameManager.Main.turnID += 1;
        if (GameManager.Main.turnID == PhotonNetwork.PlayerList.Length)
        {
            GameManager.Main.turnID = 0;
        }
        GameManager.Main.MovePiece(Vector2Int.RoundToInt(OldPosition), Vector2Int.RoundToInt(NewPosition));
    }

    [PunRPC]
    public void FinishGame(bool LocalPlayerWon, int OnlinePlayerElo)
    {
        SaveSystem.CalculateMyElo(LocalPlayerWon, OnlinePlayerElo);
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

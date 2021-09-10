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
        PhotonView.Get(this).RpcSecure("ChangeVal", RpcTarget.AllBuffered, false, ID + 1);
        PhotonView.Get(this).RpcSecure("SetSide", RpcTarget.AllBufferedViaServer, false, Random.Range(0, 2) == 1);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PhotonView.Get(this).RpcSecure("ChangeVal", RpcTarget.AllBuffered, false, ID + 1);
        }
    }

    [PunRPC]
    public void ChangeVal(int NewID)
    {
        ID = NewID;
        text.text = "ID: " + ID;
    }

    [PunRPC]
    public void SetSide(bool HostIsWhite)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GameManager.Main.localPlayerIsWhite = HostIsWhite;
            GameManager.Main.localPlayersTurn = HostIsWhite;
        }
        else
        {
            GameManager.Main.localPlayerIsWhite = !HostIsWhite;
            GameManager.Main.localPlayersTurn = !HostIsWhite;
        }
    }

    [PunRPC]
    public void FinishTurn(Vector2 OldPosition, Vector2 NewPosition)
    {
        GameManager.Main.localPlayersTurn = !GameManager.Main.localPlayersTurn;
        GameManager.Main.MovePiece(Vector2Int.RoundToInt(OldPosition), Vector2Int.RoundToInt(NewPosition));
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

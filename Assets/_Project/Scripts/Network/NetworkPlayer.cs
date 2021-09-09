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

    void Update()
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

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(0);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(0);
    }
}

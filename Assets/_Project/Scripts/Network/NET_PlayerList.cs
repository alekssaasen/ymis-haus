using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class NET_PlayerList : MonoBehaviourPunCallbacks
{
    public TMP_Text[] playerNameList;

    public void UpdatePlayerNames()
    {
        for (int i = 0; i < playerNameList.Length; i++)
        {
            if (PhotonNetwork.PlayerList.Length > i)
            {
                playerNameList[i].text = PhotonNetwork.PlayerList[i].NickName;
            }
            else
            {
                playerNameList[i].text = "";
            }
        }
    }
}

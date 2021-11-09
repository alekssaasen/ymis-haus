using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NET_GameLoopSetup : MonoBehaviourPunCallbacks
{
    public static int[] playerLookupIDs;

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            int[] IDs = new int[PhotonNetwork.PlayerList.Length];
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                IDs[i] = i;
            }

            IDs = CustomFunctions.ShuffleIntArray(IDs);

            PhotonView.Get(this).RpcSecure("StartGame", RpcTarget.AllBufferedViaServer, false, IDs);
        }
    }

    [PunRPC]
    public void StartGame(int[] IDs)
    {
        playerLookupIDs = IDs;
        GameManager.Main.localPlayerID = GetLocalPlayerID();

        GameManager.Main.StartGame();
    }

    public static int GetLocalPlayerID()
    {
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber == PhotonNetwork.PlayerList[i].ActorNumber)
            {
                return playerLookupIDs[i];
            }
        }
        return -1;
    }

    public static Player GetPlayerByID(int ID)
    {
        return PhotonNetwork.PlayerList[playerLookupIDs[ID]];
    }
}

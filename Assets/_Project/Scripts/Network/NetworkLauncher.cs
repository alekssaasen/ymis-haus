using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Realtime;
using Photon.Pun;

public class NetworkLauncher : MonoBehaviourPunCallbacks
{
    private string gameVersion = "Game1";
    public static readonly RoomOptions PUBLIC_ROOM_OPTIONS = new RoomOptions() { MaxPlayers = 2, IsVisible = true};
    public static readonly RoomOptions PRIVATE_ROOM_OPTIONS = new RoomOptions() { MaxPlayers = 2, IsVisible = false };

    public GameObject buttons;
    private string roomName;

    void Awake()
    {
        ConnectToServer();
        buttons.SetActive(false);
    }



    public void ConnectToServer()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        if (!PhotonNetwork.IsConnected)
        {
            // #Critical, we must first and foremost connect to Photon Online Server.
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = gameVersion;
        }
    }



    public void JoinRandomRoom()
    {
        if (PhotonNetwork.IsConnected)
        {
            Debug.Log("Join random room");
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            Debug.Log("Not connected to server");
        }
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Room not found, creating a new one");
        PhotonNetwork.CreateRoom(null, PUBLIC_ROOM_OPTIONS, null);
    }

    public void JoinCustomRoom(TMP_InputField RoomName)
    {
        roomName = RoomName.text.Replace(" ", "_");
        if (PhotonNetwork.IsConnected)
        {
            Debug.Log("Join custom room: " + roomName);
            PhotonNetwork.JoinRoom(roomName);
        }
        else
        {
            Debug.Log("Not connected to server");
        }
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("Room " + roomName + " not found, creating a new one");
        PhotonNetwork.CreateRoom(roomName, PRIVATE_ROOM_OPTIONS, null);
    }



    public override void OnConnectedToMaster()
    {
        Debug.Log("Server connected");
        buttons.SetActive(true);
    }
    public override void OnJoinedRoom()
    {
        Debug.Log("Room connected");
        buttons.SetActive(false);
    }
    public override void OnLeftRoom()
    {
        Debug.Log("Room disconnected");
        buttons.SetActive(true);
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Server disconnected");
        buttons.SetActive(false);
    }


    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
            {
                PhotonNetwork.LoadLevel(1);
            }
        }
    }
}

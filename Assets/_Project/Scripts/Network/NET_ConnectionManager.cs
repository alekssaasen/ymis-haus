using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;
using Photon.Realtime;

public class NET_ConnectionManager : MonoBehaviourPunCallbacks
{
    public UnityEvent OnServerConected;
    public UnityEvent OnRoomConected;
    public UnityEvent OnPlayerConected;
    public UnityEvent OnPlayerNameChanged;
    public UnityEvent OnPlayerDisconected;
    public UnityEvent OnRoomDisconected;
    public UnityEvent OnServerDisconected;

    private void Awake()
    {
        ConnectToMaster();
    }

    private void ConnectToMaster()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = Application.version;
        }
    }

    public void _ConnectToRoom()
    {
        PhotonNetwork.NickName = GUI_NameChange.GetName();
        PhotonNetwork.JoinRandomRoom();
    }

    public void _ChangeName()
    {
        PhotonNetwork.NickName = GUI_NameChange.GetName();
        PhotonView.Get(this).RpcSecure("UpdateName", RpcTarget.Others, false);
        OnPlayerNameChanged.Invoke();
    }

    [PunRPC]
    public void UpdateName()
    {
        OnPlayerNameChanged.Invoke();
    }

    public void _DisconnectFromRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    #region Network overrides

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("Server connected");
        OnServerConected.Invoke();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        base.OnJoinRandomFailed(returnCode, message);
        Debug.Log("No room found, creating a new one");
        PhotonNetwork.CreateRoom(null);
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        Debug.Log("Created room");
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("Joinded room");
        OnRoomConected.Invoke();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        Debug.Log("Player \"" + newPlayer.NickName + "\" joined");
        OnPlayerConected.Invoke();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        Debug.Log("Player \"" + otherPlayer.NickName + "\" left");
        OnPlayerDisconected.Invoke();
    }

    public override void OnLeftRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("Left room");
        OnRoomDisconected.Invoke();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        Debug.Log("Server disconnected (" + cause.ToString() + ")");
        OnServerDisconected.Invoke();

        ConnectToMaster();
    }

#endregion
}

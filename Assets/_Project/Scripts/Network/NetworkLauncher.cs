using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;

public class NetworkLauncher : MonoBehaviourPunCallbacks
{
    private string gameVersion = "Game1";
    public static readonly RoomOptions PUBLIC_ROOM_OPTIONS = new RoomOptions() { MaxPlayers = 2, IsVisible = true};
    public static readonly RoomOptions PRIVATE_ROOM_OPTIONS = new RoomOptions() { MaxPlayers = 2, IsVisible = false };

    public GameObject menuBackground;

    public GameObject mainButtons;
    public GameObject settings;
    public GameObject enterName;
    public GameObject lobbyCreate;
    public GameObject lobbyJoin;

    public TMP_Text playerList;
    public Button launchButton;
    public TMP_Dropdown gamemodeSelection;
    public Button gamemodeText;

    public TMP_Dropdown gameModeDropdown;
    public GameSettings gameModeSettings;

    void Awake()
    {
        ConnectToServer();
        gameModeSettings.CurrentGameMode = ChessGameModes.ChessEmpires;
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



    /*public void JoinRandomRoom()
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
    }*/



    public override void OnConnectedToMaster()
    {
        Debug.Log("Server connected");
        PhotonNetwork.NickName = SaveSystem.LoadPlayerData().playerName;
        mainButtons.SetActive(true);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("No room find, creating a new one");
        PhotonNetwork.CreateRoom(null, PUBLIC_ROOM_OPTIONS);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Room connected");
        mainButtons.SetActive(false);
        menuBackground.SetActive(false);
        lobbyJoin.SetActive(true);
        UpdatePlayerList();

        if (true)
        {
            gamemodeSelection.gameObject.SetActive(true);
        }
    }
    public override void OnLeftRoom()
    {
        Debug.Log("Room disconnected");
        mainButtons.SetActive(true);
        menuBackground.SetActive(true);
        lobbyJoin.SetActive(false);
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Server disconnected");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerList();
    }

#region UI

    public void JoinLobby()
    {
        if (SaveSystem.LoadPlayerData().playerName == "Player")
        {
            mainButtons.SetActive(false);
            menuBackground.SetActive(false);
            lobbyJoin.SetActive(false);
            enterName.SetActive(true);
        }
        else
        {
            PhotonNetwork.JoinRandomRoom();
        }
    }

    public void UpdatePlayerList()
    {
        string players = "";
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            players += "Player: " + PhotonNetwork.PlayerList[i].NickName;
            if (PhotonNetwork.PlayerList[i].IsMasterClient && PhotonNetwork.PlayerList[i].IsLocal)
            {
                players += " (localhost)\n";
            }
            else if (!PhotonNetwork.PlayerList[i].IsMasterClient && PhotonNetwork.PlayerList[i].IsLocal)
            {
                players += " (local)\n";
            }
            else if (PhotonNetwork.PlayerList[i].IsMasterClient && !PhotonNetwork.PlayerList[i].IsLocal)
            {
                players += " (host)\n";
            }
            else
            {
                players += "\n";
            }
        }
        playerList.text = players;
        launchButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
        gamemodeSelection.gameObject.SetActive(PhotonNetwork.IsMasterClient);
        gamemodeText.gameObject.SetActive(!PhotonNetwork.IsMasterClient);
    }

    public void LaunchGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonView.Get(this).RpcSecure("ChangeOnlineGameMode", RpcTarget.OthersBuffered, false, (int)gameModeSettings.CurrentGameMode);
            PhotonNetwork.LoadLevel(1);
        }
    }

    public void ChangeLocalGameMode(int NewGameModeID)
    {
        gameModeSettings.CurrentGameMode = (ChessGameModes)NewGameModeID;
    }

    [PunRPC]
    public void ChangeOnlineGameMode(int NewGameMode)
    {
        gameModeSettings.CurrentGameMode = (ChessGameModes)NewGameMode;
    }

    public void LeaveLobby()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void ChangeNameButton()
    {
        mainButtons.SetActive(false);
        settings.SetActive(false);
        menuBackground.SetActive(false);
        lobbyJoin.SetActive(false);
        enterName.SetActive(true);
    }

    public void ChangeName(string NewName)
    {
        PlayerSaveData savedata = SaveSystem.LoadPlayerData();
        savedata.playerName = NewName;
        PhotonNetwork.NickName = savedata.playerName;
        SaveSystem.SavePlayerData(savedata);

        mainButtons.SetActive(true);
        enterName.SetActive(false);
        menuBackground.SetActive(true);
    }

    public void SetNameSettings(TMP_InputField InputField)
    {
        InputField.text = SaveSystem.ValidateName(InputField.text);
    }

    public void GoToSettings()
    {
        mainButtons.SetActive(false);
        settings.SetActive(true);
    }
    public void ExitSettings()
    {
        mainButtons.SetActive(true);
        settings.SetActive(false);
    }

    public void DeleteData()
    {
        SaveSystem.DeletePlayerData();
    }

    public void ExitGame()
    {
        Application.Quit();
    }

#endregion

}

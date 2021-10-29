using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class NET_GameCreator : MonoBehaviour
{
    public GameSettings gameSettingsFile;
    public TMP_Text gameModeText;
    public TMP_Dropdown gameModeSelection;
    public GameObject[] hostUI;
    private GameSettings[] allGameModes;
    private int selectedID;

    public void LoadGameModes()
    {
        GameManager.ChessFigureSetInUse = Resources.LoadAll<ChessFigureSet>("FigureSets")[0];
        allGameModes = Resources.LoadAll<GameSettings>("GameModes");
        gameModeSelection.ClearOptions();

        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
        for (int i = 0; i < allGameModes.Length; i++)
        {
            options.Add(new TMP_Dropdown.OptionData(allGameModes[i].GamemodeName));
        }
        gameModeSelection.AddOptions(options);

        gameModeText.text = allGameModes[selectedID].GamemodeName;
    }

    public void SetHost()
    {
        for (int i = 0; i < hostUI.Length; i++)
        {
            hostUI[i].SetActive(PhotonNetwork.IsMasterClient);
        }
    }

    public void SendGameModeID(int ID)
    {
        PhotonView.Get(this).RpcSecure("ReceiveGameModeID", RpcTarget.AllBufferedViaServer, false, ID);
    }

    [PunRPC]
    public void ReceiveGameModeID(int ID)
    {
        selectedID = ID;
        gameModeText.text = allGameModes[selectedID].GamemodeName;

        gameSettingsFile.Deserialize(allGameModes[selectedID].Serialize());
        GameManager.GameSettingsInUse = gameSettingsFile;
    }

    public void CreateGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel(1);
        }
    }
}

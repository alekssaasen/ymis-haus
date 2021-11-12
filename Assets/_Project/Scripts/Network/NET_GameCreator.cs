using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class NET_GameCreator : MonoBehaviour
{
    public ChessFigureSet figureSetFile;
    public GameSettings gameSettingsFile;
    public TMP_Text gameModeText;
    public TMP_Dropdown gameModeSelection;
    public GameObject[] hostUI;

    private ChessFigureSet[] allFigureSets;
    private GameSettings[] allGameModes;

    public void LoadGameModes()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            allFigureSets = Resources.LoadAll<ChessFigureSet>("FigureSets");
            allGameModes = Resources.LoadAll<GameSettings>("GameModes");

            GameManager.ChessFigureSetInUse = figureSetFile;
            GameManager.GameSettingsInUse = gameSettingsFile;

            gameModeSelection.ClearOptions();

            List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
            for (int i = 0; i < allGameModes.Length; i++)
            {
                options.Add(new TMP_Dropdown.OptionData(allGameModes[i].GamemodeName));
            }

            gameModeSelection.AddOptions(options);

            gameModeSelection.value = 0;

            SendGameMode();

            for (int i = 0; i < hostUI.Length; i++)
            {
                hostUI[i].SetActive(true);
            }
        }
        else
        {
            allGameModes = Resources.LoadAll<GameSettings>("GameModes");
            allFigureSets = Resources.LoadAll<ChessFigureSet>("FigureSets");

            GameManager.ChessFigureSetInUse = figureSetFile;
            GameManager.GameSettingsInUse = gameSettingsFile;

            gameModeText.text = "";
            for (int i = 0; i < hostUI.Length; i++)
            {
                hostUI[i].SetActive(false);
            }
        }
    }

    public void SendGameMode()
    {
        PhotonView.Get(this).RpcSecure("ReceiveGameMode", RpcTarget.AllBufferedViaServer, false, gameModeSelection.value);
    }

    [PunRPC]
    public void ReceiveGameMode(int value)
    {
        gameSettingsFile = Resources.LoadAll<GameSettings>("GameModes")[value];
        gameModeText.text = gameSettingsFile.GamemodeName;
        GameManager.ChessFigureSetInUse = figureSetFile;
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

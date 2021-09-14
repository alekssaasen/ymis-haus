using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class SaveSystem
{
    public static readonly float eloChange = 250f;
    public static readonly int eloForWin = 25;

    public static readonly string SavePathToFile = "\\Saves\\PlayerSaveData.json";
    public static readonly string SavePathToFolder = "\\Saves";


    public static void CalculateMyElo(bool Winer, int OpponentElo)
    {
        PlayerSaveData savedata = LoadPlayerData();
        int elo = savedata.Elo;

        //float probabilityForPlayer1ToWin = (1f / (1f + Mathf.Pow(10f, ((MyElo - OpponentElo) / 400))));
        float probabilityForPlayer2ToWin = (1f / (1f + Mathf.Pow(10f, ((OpponentElo - elo) / 400))));

        if (Winer)
        {
            elo = Mathf.Max(0, Mathf.RoundToInt(elo + eloChange * (1f - probabilityForPlayer2ToWin)) + eloForWin);
            //OpponentElo = Mathf.Max(0, Mathf.RoundToInt(OpponentElo + eloChange * (0f - probabilityForPlayer1ToWin)));
        }
        else
        {
            elo = Mathf.Max(0, Mathf.RoundToInt(elo + eloChange * (0f - probabilityForPlayer2ToWin)));
            //OpponentElo = Mathf.Max(0, Mathf.RoundToInt(OpponentElo + eloChange * (1f - probabilityForPlayer1ToWin)) + eloForWin);
        }

        savedata.Elo = elo;
        SavePlayerData(savedata);
    }

    public static void SavePlayerData(PlayerSaveData NewPlayerData)
    {
        if (File.Exists(Application.persistentDataPath + SavePathToFile))
        {
            File.WriteAllText(Application.persistentDataPath + SavePathToFile, JsonUtility.ToJson(NewPlayerData, true));
        }
        else
        {
            Directory.CreateDirectory(Application.persistentDataPath + SavePathToFolder);
            File.WriteAllText(Application.persistentDataPath + SavePathToFile, JsonUtility.ToJson(NewPlayerData, true));
        }
    }

    public static PlayerSaveData LoadPlayerData()
    {
        if (File.Exists(Application.persistentDataPath + SavePathToFile))
        {
            return (PlayerSaveData)JsonUtility.FromJson<PlayerSaveData>(File.ReadAllText(Application.persistentDataPath + SavePathToFile));
        }
        else
        {
            Directory.CreateDirectory(Application.persistentDataPath + SavePathToFolder);
            SavePlayerData(new PlayerSaveData("Player"));
            return new PlayerSaveData("Player");
        }
    }

    public static void DeletePlayerData()
    {
        if (File.Exists(Application.persistentDataPath + SavePathToFile))
        {
            File.Delete(Application.persistentDataPath + SavePathToFile);
        }
    }

    public static string ValidateName(string Name)
    {
        Name = Name.Replace(" ", "_");
        if (Name.Length > 10)
        {
            Name = Name.Substring(0, 10);
        }
        return Name;
    }
}

[System.Serializable]
public struct PlayerSaveData
{
    public string playerName;
    public int Elo;

    public RenderQuality graphicsQuality;

    public PlayerSaveData(string NewPlayerName)
    {
        playerName = NewPlayerName;
        Elo = 1000;

        graphicsQuality = RenderQuality.High;
    }
}

public enum RenderQuality
{
    Ultra, High, Medium, Low, Potato
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SettingsChanger : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    public GameObject postProcessing;

    private void Start()
    {
        LoadSettings();
    }

    public void LoadSettings()
    {
        PlayerSaveData playersavedata = SaveSystem.LoadPlayerData();
        QualitySettings.SetQualityLevel((int)playersavedata.graphicsQuality);

        if (dropdown != null)
        {
            dropdown.SetValueWithoutNotify((int)playersavedata.graphicsQuality);
        }

        if ((int)playersavedata.graphicsQuality <= 1)
        {
            postProcessing.SetActive(true);
        }
        else
        {
            postProcessing.SetActive(false);
        }
    }

    public void UpdateSettings(int NewRenderQuality)
    {
        PlayerSaveData playersavedata = SaveSystem.LoadPlayerData();
        playersavedata.graphicsQuality = (RenderQuality)NewRenderQuality;
        QualitySettings.SetQualityLevel((int)playersavedata.graphicsQuality);
        SaveSystem.SavePlayerData(playersavedata);

        if ((int)playersavedata.graphicsQuality <= 1)
        {
            postProcessing.SetActive(true);
        }
        else
        {
            postProcessing.SetActive(false);
        }
    }
}

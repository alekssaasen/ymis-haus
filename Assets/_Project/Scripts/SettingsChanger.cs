using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using TMPro;

public class SettingsChanger : MonoBehaviour
{
    public UniversalRenderPipelineAsset[] renderAssets;
    public TMP_Dropdown dropdown;

    private void Start()
    {
        LoadSettings();
    }

    public void LoadSettings()
    {
        PlayerSaveData playersavedata = SaveSystem.LoadPlayerData();
        QualitySettings.renderPipeline = renderAssets[(int)playersavedata.graphicsQuality];
        dropdown.SetValueWithoutNotify((int)playersavedata.graphicsQuality);
    }

    public void UpdateSettings(int NewRenderQuality)
    {
        PlayerSaveData playersavedata = SaveSystem.LoadPlayerData();
        playersavedata.graphicsQuality = (RenderQuality)NewRenderQuality;
        QualitySettings.renderPipeline = renderAssets[(int)playersavedata.graphicsQuality];
        SaveSystem.SavePlayerData(playersavedata);
    }
}

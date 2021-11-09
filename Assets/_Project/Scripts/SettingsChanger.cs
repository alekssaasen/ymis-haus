using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsChanger : MonoBehaviour
{
    public GameObject postProcessing;
    public Slider graphicsQualitySlider;

    private void Start()
    {
        UpdateSettings();
    }

    private void UpdateSettings()
    {
        if (!PlayerPrefs.HasKey("GraphicsQuality"))
        {
            PlayerPrefs.SetInt("GraphicsQuality", 3);
            PlayerPrefs.Save();
        }

        QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("GraphicsQuality"));
        if (graphicsQualitySlider != null) { graphicsQualitySlider.value = PlayerPrefs.GetInt("GraphicsQuality"); }

        if (postProcessing != null && PlayerPrefs.GetInt("GraphicsQuality") > 1)
        {
            postProcessing.SetActive(true);
        }
        else
        {
            postProcessing.SetActive(false);
        }
    }

    public void UpdateGraphics(float GraphicsQuality)
    {
        PlayerPrefs.SetInt("GraphicsQuality", Mathf.RoundToInt(GraphicsQuality));
        PlayerPrefs.Save();
        UpdateSettings();
    }

    /*public void UpdateVolume(float GameVolume)
    {
        PlayerPrefs.SetFloat("GameVolume", GameVolume);
        PlayerPrefs.Save();
        UpdateSettings();
    }*/
}

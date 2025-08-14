using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResolutionSelect : MonoBehaviour
{
    public TMP_Dropdown resolutionDropdown;

    private static readonly (string label, Resolution resolution)[] resolutions = new (string, Resolution)[]
    {
        ("1280 x 720",  new Resolution { width = 1280, height = 720 }),
        ("1280 x 800",  new Resolution { width = 1280, height = 800 }),
        ("1440 x 900",  new Resolution { width = 1440, height = 900 }),
        ("1600 x 900",  new Resolution { width = 1600, height = 900 }),
        ("1680 x 1050", new Resolution { width = 1680, height = 1050 }),
        ("1920 x 1080", new Resolution { width = 1920, height = 1080 }),
        ("1920 x 1200", new Resolution { width = 1920, height = 1200 }),
        ("2048 x 1280", new Resolution { width = 2048, height = 1280 }),
        ("2560 x 1440", new Resolution { width = 2560, height = 1440 }),
        ("2560 x 1600", new Resolution { width = 2560, height = 1600 }),
        ("2880 x 1800", new Resolution { width = 2880, height = 1800 }),
        ("3480 x 2160", new Resolution { width = 3480, height = 2160 })
    };

    private List<Resolution> resolutionList = new List<Resolution>();
    private int optimalResolutionIndex = 0;
    private int selectedResolutionIndex = 0;

    private const string ResolutionPrefKey = "ResolutionIndex";

    private void Start()
    {
        int savedIndex = PlayerPrefs.GetInt(ResolutionPrefKey, -1);
        InitializeResolutionDropdown(savedIndex);

        resolutionDropdown.onValueChanged.AddListener(OnResolutionSelected);
    }

    private void InitializeResolutionDropdown(int savedIndex)
    {
        resolutionList.Clear();
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        for (int i = 0; i < resolutions.Length; i++)
        {
            var (label, resolution) = resolutions[i];
            resolutionList.Add(resolution);

            // 현재 화면 해상도와 비교해 최적 인덱스 지정
            if (resolution.width == Screen.currentResolution.width &&
                resolution.height == Screen.currentResolution.height)
            {
                optimalResolutionIndex = i;
                label += " *";
            }

            options.Add(label);
        }

        resolutionDropdown.AddOptions(options);

        if (savedIndex >= 0 && savedIndex < resolutionList.Count)
            selectedResolutionIndex = savedIndex;
        else
            selectedResolutionIndex = optimalResolutionIndex;

        resolutionDropdown.value = selectedResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        ApplyResolution();
    }

    public void OnResolutionSelected(int index)
    {
        selectedResolutionIndex = index;
        ApplyResolution();
    }

    public void ApplyResolution()
    {
        Resolution resolution = resolutionList[selectedResolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreenMode);

        PlayerPrefs.SetInt(ResolutionPrefKey, selectedResolutionIndex);
        PlayerPrefs.Save();
    }

    public void ResetResolution()
    {
        selectedResolutionIndex = optimalResolutionIndex;
        resolutionDropdown.value = selectedResolutionIndex;
        resolutionDropdown.RefreshShownValue();
        ApplyResolution();
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplayModeSelect : MonoBehaviour
{
    public TMP_Dropdown displayModeDropdown;

    private readonly string[] displayModeLabels = {
        "Full Screen",
        "Windowed Full Screen",
        "Window"
    };

    private readonly FullScreenMode[] displayModes = {
        FullScreenMode.FullScreenWindow,   // Full Screen
        FullScreenMode.MaximizedWindow,    // Windowed Full Screen
        FullScreenMode.Windowed            // Window
    };

    private int selectedDisplayModeIndex = 0;

    private const string DisplayModePrefKey = "DisplayModeIndex";
    private const int DefaultDisplayModeIndex = 0; // 기본값: Full Screen

    private void Start()
    {
        int savedIndex = PlayerPrefs.GetInt(DisplayModePrefKey, -1);
        DisplayModeInitialize(savedIndex);
        displayModeDropdown.onValueChanged.AddListener(OnDisplayModeSelected);
    }

    #region Initialize
    private void DisplayModeInitialize(int savedIndex)
    {
        displayModeDropdown.ClearOptions();
        displayModeDropdown.AddOptions(new List<string>(displayModeLabels));

        if (savedIndex >= 0 && savedIndex < displayModes.Length)
        {
            selectedDisplayModeIndex = savedIndex;
        }
        else
        {
            for (int i = 0; i < displayModes.Length; i++)
            {
                if (Screen.fullScreenMode == displayModes[i])
                {
                    selectedDisplayModeIndex = i;
                    break;
                }
            }
        }

        displayModeDropdown.value = selectedDisplayModeIndex;
        displayModeDropdown.RefreshShownValue();
        Screen.fullScreenMode = displayModes[selectedDisplayModeIndex];
    }
    #endregion

    private void OnDisplayModeSelected(int index)
    {
        selectedDisplayModeIndex = index;
        ApplyDisplayMode();
    }

    public void ApplyDisplayMode()
    {
        Screen.fullScreenMode = displayModes[selectedDisplayModeIndex];
        PlayerPrefs.SetInt(DisplayModePrefKey, selectedDisplayModeIndex);
        PlayerPrefs.Save();
    }

    public void ResetDisplayMode()
    {
        selectedDisplayModeIndex = DefaultDisplayModeIndex;

        displayModeDropdown.value = selectedDisplayModeIndex;
        displayModeDropdown.RefreshShownValue();

        Screen.fullScreenMode = displayModes[selectedDisplayModeIndex];

        PlayerPrefs.SetInt(DisplayModePrefKey, selectedDisplayModeIndex);
        PlayerPrefs.Save();
    }
}

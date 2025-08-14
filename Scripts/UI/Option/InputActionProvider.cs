using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public static class InputActionProvider
{
    private static PlayerInput _sharedInput;

    public static InputActionAsset InputActions
    {
        get
        {
            if (_sharedInput == null)
                _sharedInput = new PlayerInput();

            return _sharedInput.asset;
        }
    }

    public static void LoadFromPrefs()
    {
        if (PlayerPrefs.HasKey("keybindings"))
        {
            string rebinds = PlayerPrefs.GetString("keybindings");
            InputActions.LoadBindingOverridesFromJson(rebinds);
        }
    }

    public static void SaveToPrefs()
    {
        string rebinds = InputActions.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString("keybindings", rebinds);
        PlayerPrefs.Save();
    }
}

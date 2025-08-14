using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class KeyBindManager : MonoBehaviour
{
    public static KeyBindManager Instance;

    private InputManager inputManager;
    private InputActionAsset inputActions;

    [SerializeField] private OptionMenuUI optionMenuUI; 

    public KeyBindButton[] keyBindButtons;

    public Button backBtn;
    public Button resetBtn;

    protected virtual void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        backBtn?.onClick.AddListener(Back);
        resetBtn?.onClick.AddListener(ResetKeys);

        if (GameManager.Instance != null && GameManager.Instance.CurrentPlayer != null)
        {
            inputManager = GameManager.Instance.CurrentPlayer.InputManager;
            inputActions = inputManager.GetInputActions().asset;
        }
        else
        {
            inputActions = InputActionProvider.InputActions;
            InputActionProvider.LoadFromPrefs(); // 저장된 키 설정 불러오기
        }

        KeyBindInitialize();
    }

    #region Key Initialize
    private void KeyBindInitialize()
    {
        foreach (var keyBindButton in keyBindButtons)
        {
            if (string.IsNullOrEmpty(keyBindButton.actionName))
                continue;

            var action = inputActions.FindAction(keyBindButton.actionName, true);

            if (action == null)
            {
                Debug.LogWarning($"'{keyBindButton.actionName}' 액션을 찾을 수 없습니다.");
                continue;
            }

            keyBindButton.inputAction = action;
            keyBindButton.UpdateKeyDisplay();
        }
    }
    #endregion

    #region Button
    private void Back()
    {
        optionMenuUI.optionWindow.SetActive(true);
        optionMenuUI.keyBindWindow.SetActive(false);
    }

    public void ResetKeys()
    {
        foreach (var keyBindButton in keyBindButtons)
        {
            keyBindButton.ResetKey();
        }

        SaveKeys();
    }
    #endregion

    #region Save & Load

    public void SaveKeys()
    {
        SaveKeyBindings(inputActions);
    }

    public void SaveKeyBindings(InputActionAsset actions)
    {
        string rebinds = actions.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString("keybindings", rebinds);
        PlayerPrefs.Save();

        if (InputActionProvider.InputActions != inputActions)
            InputActionProvider.InputActions.LoadBindingOverridesFromJson(rebinds);
    }

    public void LoadKeyBindings(InputActionAsset actions)
    {
        if (PlayerPrefs.HasKey("keybindings"))
        {
            string rebinds = PlayerPrefs.GetString("keybindings");
            actions.LoadBindingOverridesFromJson(rebinds);
        }

        foreach (var keyBindButton in keyBindButtons)
        {
            keyBindButton.UpdateKeyDisplay();
        }
    }
    #endregion
}
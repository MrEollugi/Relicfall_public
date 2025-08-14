using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class KeyBindButton : MonoBehaviour
{
    public Button bindButton;
    public TextMeshProUGUI keyName;

    public InputAction inputAction;
    public string actionName;
    public int bindingIndex = 0;

    private InputActionRebindingExtensions.RebindingOperation rebindOp;

    private void OnEnable()
    {
        bindButton?.onClick.AddListener(StartRebind);
        UpdateKeyDisplay();
    }

    private void OnDisable()
    {
        bindButton?.onClick.RemoveListener(StartRebind);
        rebindOp?.Dispose();
    }

    private void StartRebind()
    {
        if (inputAction == null || rebindOp != null || bindingIndex >= inputAction.bindings.Count)
            return;

        keyName.text = "Waiting";

        inputAction.Disable();

        rebindOp = inputAction.PerformInteractiveRebinding(bindingIndex)
            .WithControlsExcluding("<Pointer>/position")
            .WithControlsExcluding("<Pointer>/delta")
            .OnMatchWaitForAnother(0.1f)
            .OnComplete(operation =>
            {
                rebindOp.Dispose();
                rebindOp = null;

                inputAction.Enable();
                UpdateKeyDisplay();

                if (KeyBindManager.Instance != null)
                    KeyBindManager.Instance.SaveKeys();
            })
            .Start();
    }

    public void UpdateKeyDisplay()
    {
        if (inputAction == null || bindingIndex >= inputAction.bindings.Count)
        {
            keyName.text = "N/A";
            return;
        }

        var binding = inputAction.bindings[bindingIndex];
        var path = binding.overridePath ?? binding.effectivePath;

        if (string.IsNullOrEmpty(path))
        {
            keyName.text = "Unbound";
        }
        else
        {
            keyName.text = InputControlPath.ToHumanReadableString(
                path,
                InputControlPath.HumanReadableStringOptions.OmitDevice
            );
        }
    }

    public void ResetKey()
    {
        if (inputAction == null || bindingIndex >= inputAction.bindings.Count)
            return;

        inputAction.RemoveBindingOverride(bindingIndex);
        UpdateKeyDisplay();
    }
}

using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InputUI : MonoBehaviour
{
    public static InputUI Instance;

    public bool IsActive => gameObject.activeSelf;

    public TMP_InputField inputField;
    public Button confirmButton;
    public Button cancelButton;

    private Action<int> onConfirmCallback;
    private int maxCount;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        gameObject.SetActive(false);
    }

    private void Start()
    {
        confirmButton.onClick.AddListener(Confirm);
        cancelButton.onClick.AddListener(Hide);
        inputField.onValueChanged.AddListener(OnInputFieldValueChanged);
    }

    private void OnInputFieldValueChanged(string newText)
    {
        if (int.TryParse(newText, out int count))
        {
            if (count > maxCount)
            {
                // �Էµ� ���ڰ� maxCount�� �ʰ��ϸ�, �Է� �ʵ� �ؽ�Ʈ�� maxCount�� �����մϴ�.
                inputField.text = maxCount.ToString();
                
                inputField.caretPosition = inputField.text.Length;
            }
            else if (count < 0)
            {
                inputField.text = "0";
                inputField.caretPosition = inputField.text.Length;
            }
        }
    }

    public void Confirm()
    {
        if (int.TryParse(inputField.text, out int count))
        {
            if (count > maxCount)
            {
                count = maxCount;  // �ִ밪���� ����
            }

            if (count > 0)
            {
                onConfirmCallback?.Invoke(count);
            }
        }

        Hide();
    }

    public void Show(Action<int> onConfirm, int maxCount)
    {
        gameObject.SetActive(true);
        inputField.text = "";
        onConfirmCallback = onConfirm;
        this.maxCount = maxCount;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}

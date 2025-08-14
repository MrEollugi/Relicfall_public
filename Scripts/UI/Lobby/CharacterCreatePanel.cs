using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCreatePanel : MonoBehaviour
{
    public TMP_InputField nicknameInput;
    public Button confirmButton;
    public Button cancelButton;
    private string slotId;

    public SoloSlotPanel soloSlotPanel;
    public static CharacterCreatePanel Instance;

    void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }

    public static void Open(string slotId)
    {
        Instance.slotId = slotId;
        Instance.nicknameInput.text = "";
        Instance.gameObject.SetActive(true);
    }

    public void OnConfirmClick()
    {
        string nickname = nicknameInput.text.Trim();
        if (string.IsNullOrEmpty(nickname))
        {
            return;
        }

        var player = new PlayerData();
        player.Nickname = nickname;
        player.JobId = "job_knight";
        player.Level = 1;

        new SaveManager().SavePlayer(player, slotId, GameMode.Single);

        gameObject.SetActive(false);

        if (soloSlotPanel != null)
            soloSlotPanel.RefreshSlots();

        GameManager.Instance.SetSaveSlot(slotId, GameMode.Single);
        UnityEngine.SceneManagement.SceneManager.LoadScene("HubScene");
    }

    public void OnCancelClick()
    {
        gameObject.SetActive(false);
    }
}

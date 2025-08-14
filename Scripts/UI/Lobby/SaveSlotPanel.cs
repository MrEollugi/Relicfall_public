using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveSlotPanel : MonoBehaviour
{
    public TextMeshProUGUI NameText;
    public TextMeshProUGUI LevelText;
    public TextMeshProUGUI GoldText;
    public TextMeshProUGUI PlayTimeText;
    public Button DeleteButton;

    private string slotId;

    public void SetSlotData(string slotId, PlayerData player, WorldData world)
    {
        this.slotId = slotId;

        if (player == null)
        {
            NameText.text = "NEW GAME";
            LevelText.text = "-";
            GoldText.text = "-";
            PlayTimeText.text = "-";
            DeleteButton.interactable = false;
            return;
        }

        if (!string.IsNullOrEmpty(player.Nickname))
        {
            NameText.text = player.Nickname;
            LevelText.text = $"Lv. {player.Level}";
            GoldText.text = world?.TotalGold.ToString() ?? "-";
            PlayTimeText.text = PlayTimeUtils.FormatPlayTime(player.PlayTimeSeconds);
            DeleteButton.interactable = true;
        }
        else
        {
            NameText.text = "NEW GAME";
            LevelText.text = "-";
            GoldText.text = "-";
            PlayTimeText.text = "-";
            DeleteButton.interactable = false;
        }
    }

    public void OnClick()
    {
        var player = new SaveManager().LoadPlayer(slotId, GameMode.Single);

        if (player == null || string.IsNullOrEmpty(player.Nickname))
        {
            // 닉네임(캐릭터 생성) 입력 UI 띄우기
            CharacterCreatePanel.Open(slotId);
        }
        else
        {
            // 이어하기(기존 플레이어)
            GameManager.Instance.SetSaveSlot(slotId, GameMode.Single);
            GameManager.Instance._loadingManager.StartLoad("HubScene");
            //UnityEngine.SceneManagement.SceneManager.LoadScene("HubScene");
        }
    }

    public void OnDeleteClick()
    {
        // 1. 정말 삭제할지 확인 팝업 (간단히 예시)
        if (!ConfirmDelete())
            return;

        // 2. delete save file
        var saveManager = new SaveManager();
        string playerPath = System.IO.Path.Combine(
            Application.persistentDataPath, $"single_player_{slotId}.json");
        string worldPath = System.IO.Path.Combine(
            Application.persistentDataPath, $"single_world_{slotId}.json");
        if (System.IO.File.Exists(playerPath))
            System.IO.File.Delete(playerPath);
        if (System.IO.File.Exists(worldPath))
            System.IO.File.Delete(worldPath);

        // 3. 슬롯 정보 갱신(새 게임 상태로 표시)
        SetSlotData(slotId, new PlayerData(), new WorldData());
        transform.parent.GetComponent<SoloSlotPanel>().RefreshSlots();
    }

    // 아주 간단한 팝업(진짜 UI는 별도 구현)
    private bool ConfirmDelete()
    {
#if UNITY_EDITOR
        return UnityEditor.EditorUtility.DisplayDialog("Delete Slot", "Are You Sure?", "Yes", "No");
#else
    // 실제 게임에선 커스텀 팝업 창/확인창 등으로 구현 필요
    return true; // (임시: 무조건 삭제) → 실제 구현 시엔 반드시 팝업 적용!
#endif
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyMenuUI : MonoBehaviour
{
    public TextMeshProUGUI[] menuItems;   // 0:Solo 1:Together 2:Option 3:종료
    public Color normalColor = Color.white;
    public Color highlightColor = new Color(1f, 0.55f, 0.23f);

    private int currentIndex = 0;

    #region Save Slot Field

    public GameObject slotSelectPanel;
    public SoloSlotPanel soloSlotPanel;
    public GameObject multiSlotSelectPanel;

    private GameMode selectedMode = GameMode.Single;

    #endregion

    #region Option

    public GameObject optionPanel;
    [SerializeField] private OptionMenuUI optionMenuUI; 

    #endregion

    void Start()
    {
        UpdateMenuUI();
        slotSelectPanel.SetActive(false);
    }

    void Update()
    {
        // 아래키
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentIndex = (currentIndex + 1) % menuItems.Length;
            UpdateMenuUI();
        }
        // 위키
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentIndex = (currentIndex - 1 + menuItems.Length) % menuItems.Length;
            UpdateMenuUI();
        }
        // 엔터
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            OnMenuSelect(currentIndex);
        }
    }

    void UpdateMenuUI()
    {
        for (int i = 0; i < menuItems.Length; i++)
        {
            if (i == currentIndex)
            {
                menuItems[i].color = highlightColor;
                menuItems[i].text = $"> {GetMenuText(i)}";
            }
            else
            {
                menuItems[i].color = normalColor;
                menuItems[i].text = $"  {GetMenuText(i)}";
            }
        }
    }

    string GetMenuText(int idx)
    {
        // 원하는 텍스트로 반환
        switch (idx)
        {
            case 0: return "Single";
            case 1: return "Multiplay";
            case 2: return "Option";
            case 3: return "Exit";
            default: return "";
        }
    }

    void OnMenuSelect(int idx)
    {
        switch (idx)
        {
            case 0:
                selectedMode = GameMode.Single;
                ShowSlotSelectPanel();
                break;
            case 1:
                selectedMode = GameMode.Multi;
                Debug.Log("멀티플레이는 준비 중입니다.");
                //ShowMultiSlotSelectPanel();
                break;
            case 2:
                Debug.Log("설정 선택");
                // 옵션 패널 띄우기
                ShowOptionPanel();
                break;
            case 3:
                Debug.Log("종료 선택");
                #if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
                #else
                    Application.Quit();
                #endif
                break;
        }
    }

    #region Save Slots

    void ShowSlotSelectPanel()
    {
        slotSelectPanel.SetActive(true);
        soloSlotPanel.RefreshSlots();
    }

    //void ShowMultiSlotSelectPanel()
    //{
    //    multiSlotSelectPanel.SetActive(true);
    //    for (int i = 0; i < multiSlotTexts.Length; i++)
    //    {
    //        string userId = GetUserIdForSlot(i); // 실제 구현 필요 (유저ID, 닉네임 등)
    //        var playerData = new SaveManager().LoadPlayer(userId, GameMode.Multi);
    //        multiSlotTexts[i].text = $"User {userId}\nLv.{playerData.Level} Gold: ...";
    //    }
    //}

    #endregion

    #region OptionPanel

    void ShowOptionPanel()
    {
        optionPanel.SetActive(true);
        optionMenuUI.optionWindow.SetActive(true);
        optionMenuUI.keyBindWindow.SetActive(false);
    }

    #endregion

    public void OnStartClick() => OnMenuSelect(0);   // 게임시작
    public void OnOptionClick() => OnMenuSelect(2);  // 옵션
    public void OnExitClick() => OnMenuSelect(3);    // 종료
}

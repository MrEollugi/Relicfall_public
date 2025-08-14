using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OptionMenuUI : MonoBehaviour
{
    [Header("옵션 패널")]
    public GameObject menuWindow;
    public GameObject optionWindow;
    public GameObject keyBindWindow;
    public GameObject ReturnWindow;

    [Header("메뉴 관련 버튼")]
    public Button resumeBtn;
    public Button optionBtn;
    public Button quitBtn;

    [Header("설정 관련 버튼")]
    public Button keyBindingBtn;
    public Button applyBtn;
    public Button backBtn;

    [Header("메인으로 돌아가기 관련 버튼")]
    public Button confirmBtn;
    public Button cancelBtn;

    [Header("디스플레이 설정")]
    public ResolutionSelect resolutionSelect;
    public DisplayModeSelect displayModeSelect;

    public bool IsOpen => gameObject.activeInHierarchy;

    private void Start()
    {
        if (menuWindow != null)
            gameObject.SetActive(false);

        resumeBtn?.onClick.AddListener(Resume);
        optionBtn?.onClick.AddListener(Option);
        quitBtn?.onClick.AddListener(Quit);

        keyBindingBtn?.onClick.AddListener(KeyBind);
        applyBtn?.onClick.AddListener(ApplySettings);
        backBtn?.onClick.AddListener(Back);

        confirmBtn?.onClick.AddListener(ReturnToMain);
        cancelBtn?.onClick.AddListener(Cancel);
    }

    #region 버튼 기능들

    #region 메뉴

    private void Resume()
    {
        EventSystem.current.SetSelectedGameObject(null);

        if (ButtonEffect.currentlySelected != null)
        {
            ButtonEffect.currentlySelected.Deselect();
            ButtonEffect.currentlySelected = null;
        }

        menuWindow.SetActive(false);
        optionWindow.SetActive(false);
        keyBindWindow.SetActive(false);
        gameObject.SetActive(false);
    }

    private void Option()
    {
        menuWindow.SetActive(false);
        optionWindow.SetActive(true);
    }

    private void Quit()
    {
        menuWindow.SetActive(false);
        ReturnWindow.SetActive(true);
    }

    #endregion

    #region 설정

    private void KeyBind()
    {
        optionWindow.SetActive(false);
        keyBindWindow.SetActive(true);
    }

    public void ApplySettings()
    {
        resolutionSelect.ApplyResolution();
        displayModeSelect.ApplyDisplayMode();
    }

    private void Back()
    {
        if (menuWindow == null)
        {
            EventSystem.current.SetSelectedGameObject(null);

            if (ButtonEffect.currentlySelected != null)
            {
                ButtonEffect.currentlySelected.Deselect();
                ButtonEffect.currentlySelected = null;
            }

            gameObject.SetActive(false);
            return;
        }

        menuWindow.SetActive(true);
        optionWindow.SetActive(false);
    }

    #endregion

    #region 타이틀로 돌아가기

    public void ReturnToMain()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("LobbyScene");
    }

    public void Cancel()
    {
        menuWindow.SetActive(true);
        ReturnWindow.SetActive(false);
    }

    #endregion

    #endregion

    public void Toggle()    // ESC를 통한 UI 토글
    {
        menuWindow.SetActive(true);
        optionWindow.SetActive(false);
        keyBindWindow.SetActive(false);

        if (IsOpen)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }
    }
}
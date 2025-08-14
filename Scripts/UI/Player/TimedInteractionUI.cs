using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum TimedInteractionUIPositionType
{
    Center,
    Mouse,
    World
}

public class TimedInteractionUI : MonoBehaviour
{
    public Image ringImage;
    public TextMeshProUGUI keyText;
    public CanvasGroup group;

    // 호출 시 초기화 및 표시 (위치 지정 가능)
    public void Show(float totalTime, Vector3 worldPos, TimedInteractionUIPositionType positionType = TimedInteractionUIPositionType.Center)
    {
        group.alpha = 1;
        SetProgress(0f);
        keyText.text = "F";

        var rect = GetComponent<RectTransform>();

        switch (positionType)
        {
            case TimedInteractionUIPositionType.Center:
                rect.anchoredPosition = Vector2.zero;
                break;
            case TimedInteractionUIPositionType.Mouse:
                transform.position = Input.mousePosition;
                break;
            case TimedInteractionUIPositionType.World:
                transform.position = Camera.main.WorldToScreenPoint(worldPos);
                break;
        }
    }

    // 게이지 업데이트 (0.0 ~ 1.0)
    public void SetProgress(float progress)
    {
        ringImage.fillAmount = Mathf.Clamp01(progress);
    }

    public void Hide()
    {
        group.alpha = 0;
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    public static ButtonEffect currentlySelected;

    [Header("UI References")]
    public TextMeshProUGUI label;

    [Header("Colors")]
    public Color normalColor = Color.white;
    public Color hoverColor = Color.black;

    [Header("Fade Settings")]
    public float fadeDuration = 0.2f;

    private Coroutine fadeCoroutine;
    private bool isSelected = false;

    private void OnEnable()
    {
        SetColor(normalColor);
        isSelected = false;

        // 선택된 버튼이 사라지면 해제
        if (currentlySelected == this)
            currentlySelected = null;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isSelected)
            StartFadeToColor(hoverColor);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isSelected)
            StartFadeToColor(normalColor);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // 이전 선택된 버튼 초기화
        if (currentlySelected != null && currentlySelected != this)
        {
            currentlySelected.Deselect();
        }

        currentlySelected = this;
        isSelected = true;
        StartFadeToColor(hoverColor);
    }

    public void Deselect()
    {
        isSelected = false;

        if (!gameObject.activeInHierarchy)
        {
            if (label != null)
                label.color = normalColor;

            return;
        }

        StartFadeToColor(normalColor);
    }

    private void SetColor(Color color)
    {
        if (label != null)
            label.color = color;
    }

    private void StartFadeToColor(Color targetColor)
    {
        if (!gameObject.activeInHierarchy)
        {
            if (label != null)
                label.color = targetColor;
            return;
        }

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeTextColor(targetColor));
    }

    private IEnumerator FadeTextColor(Color targetColor)
    {
        Color startColor = label.color;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            label.color = Color.Lerp(startColor, targetColor, elapsed / fadeDuration);
            yield return null;
        }

        label.color = targetColor;
    }
}

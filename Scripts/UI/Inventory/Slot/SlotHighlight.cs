using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SlotHighlight : MonoBehaviour
{
    private Outline outline;

    private Color previousColor;
    private Color highlightColor;

    private void Awake()
    {
        DOTween.Init();

        outline = GetComponent<Outline>();

        previousColor = new Color32(34, 32, 52, 255);
        highlightColor = new Color32(176, 176, 189, 255);
    }

    public void HighlightOn()
    {
        if (outline != null)
        {
            outline.DOKill(true);
            outline.DOColor(highlightColor, 0.2f).SetEase(Ease.InOutSine);
        }
    }

    public void HighlightOff()
    {
        if (outline != null)
        {
            outline.DOKill(true);
            outline.DOColor(previousColor, 0.2f).SetEase(Ease.InOutSine);
        }
    }
}

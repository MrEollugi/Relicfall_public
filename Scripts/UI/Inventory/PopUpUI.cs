using UnityEngine;
using DG.Tweening;
using System;

public class PopUpUI : MonoBehaviour
{
    private void Awake()
    {
        DOTween.Init();

        transform.localScale = Vector3.one * 0.1f;
    }

    public void Show()
    {
        gameObject.SetActive(true);

        var seq = DOTween.Sequence();

        seq.Append(transform.DOScale(1.1f, 0.2f));
        seq.Append(transform.DOScale(1.0f, 0.1f));

        seq.Play();
    }
}

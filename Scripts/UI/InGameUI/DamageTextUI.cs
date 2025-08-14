using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageTextUI : MonoBehaviour
{
    public TextMeshProUGUI damageText;

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void SetDamage(float amount, bool isCrit = false)
    {
        damageText.text = Mathf.RoundToInt(amount).ToString();
        damageText.color = isCrit ? Color.yellow : Color.white;

        transform.localPosition += new Vector3(Random.Range(-0.2f, 0.2f), 0, 0);

        Vector3 targetPos = transform.position + new Vector3(0f, 1.0f, 1.0f);
        transform.DOMove(targetPos, 1.0f).SetEase(Ease.OutCubic);

        canvasGroup.alpha = 1f;
        canvasGroup.DOFade(0f, 1.0f)
            .SetEase(Ease.InQuad)
            .OnComplete(() => Destroy(gameObject));
    }
}

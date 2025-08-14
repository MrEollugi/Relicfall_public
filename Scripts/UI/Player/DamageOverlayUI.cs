using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageOverlayUI : MonoBehaviour
{
    public Image overlayImage;
    public float flashDuration = 0.2f;
    public float fadeSpeed = 3f;

    private float overlayAlpha = 0f;

    void Update()
    {
        if (overlayAlpha > 0f)
        {
            overlayAlpha -= Time.deltaTime * fadeSpeed;
            overlayAlpha = Mathf.Max(0f, overlayAlpha);
            SetOverlayAlpha(overlayAlpha);
        }
    }

    public void Flash()
    {
        overlayAlpha = 0.5f;
        SetOverlayAlpha(overlayAlpha);
    }

    private void SetOverlayAlpha(float a)
    {
        var c = overlayImage.color;
        c.a = a;
        overlayImage.color = c;
    }
}

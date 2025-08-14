using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Afterimage : MonoBehaviour
{
    private SpriteRenderer sr;
    private Color color;
    public float fadeSpeed = 2f;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        color = sr.color;
    }

    void Update()
    {
        color.a -= fadeSpeed * Time.deltaTime;
        sr.color = color;
        if (color.a <= 0f)
            Destroy(gameObject);
    }

    public void SetSprite(Sprite sprite, Color baseColor, bool flipX)
    {
        sr.sprite = sprite;
        sr.flipX = flipX;
        sr.color = baseColor;
        color = baseColor;
    }
}

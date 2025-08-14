using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightWeaponController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer weaponSprite;
    [SerializeField] private Transform visualTransform;

    private Vector3 guardPos = new Vector3(0.25f, 0.33f, 0.22f);     // 예시: 오른쪽/앞
    private Vector3 normalPos = new Vector3(-0.25f, 0.33f, 0.22f);   // 원래 위치

    public void SetGuardPosition(bool isGuard)
    {
        if (visualTransform != null)
            visualTransform.localPosition = isGuard ? guardPos : normalPos;
    }

    public void UpdateShield(bool isMouseRight)
    {
        Vector3 scale = Vector3.one;
        scale.x = isMouseRight ? 1 : -1;
        transform.localScale = scale;
        transform.localRotation = Quaternion.identity;
    }

    public void SetSide(bool isMouseRight)
    {
        // 방패는 검 반대쪽 위치
        transform.localPosition = isMouseRight ? new Vector3(0f, 0f, 0f) : new Vector3(0f, 0f, 0f);
    }

    public void SetWeaponSprite(Sprite sprite)
    {
        weaponSprite.sprite = sprite;
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftWeaponController : MonoBehaviour
{
    [SerializeField] private Transform weaponVisual;
    public Transform WeaponVisual => weaponVisual;

    [SerializeField] private SpriteRenderer weaponSprite;
    public SpriteRenderer WeaponSprite => weaponSprite;

    [SerializeField] private Animator animator;
    public Animator Animator => animator;
    public Transform MuzzlePoint => transform.Find("LeftWeapon_Visual/MuzzlePoint");

    public void UpdateWeapon(Vector3 mouseScreenPos, Vector3 playerScreenPos, bool isMouseRight, bool isMouseUpper)
    {
        Vector2 dir = mouseScreenPos - playerScreenPos;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        if (isMouseRight)
        {
            angle += 0f;
            weaponSprite.flipY = false;
        }
        else
        {
            angle -= 00f;
            weaponSprite.flipY = true;
        }
        //if(!isMouseRight)
        //{
        //    weaponSprite.flipX = true;
        //    weaponSprite.flipY = false;
        //}
        //else
        //{
        //    weaponSprite.flipX = false;
        //    weaponSprite.flipY = true;
        //}

        // Z축 회전
        weaponVisual.localRotation = Quaternion.Euler(35f, 0, angle);

        var mpb = new MaterialPropertyBlock();
        weaponSprite.GetPropertyBlock(mpb);
        mpb.SetFloat("_FlipY", weaponSprite.flipY ? 1f : 0f);
        weaponSprite.SetPropertyBlock(mpb);
    }

    public void SetSide(bool isMouseRight)
    {
        transform.localPosition = isMouseRight ? new Vector3(0f, 0f, 0f) : new Vector3(-0.7f, 0f, 0f);

        if (weaponSprite != null)
        {
            bool flipY = weaponSprite.flipY;
            if (MuzzlePoint != null)
            {
                Vector3 basePos = new Vector3(1.3f, 0.22f, 0); // 오른손(오른쪽) 기준값
                MuzzlePoint.localPosition = flipY ? new Vector3(basePos.x, -basePos.y, basePos.z) : basePos;
            }
        }

    }

    public void SetWeaponSprite(Sprite sprite)
    {
        weaponSprite.sprite = sprite;
    }

    #region Swing

    public void StartSwingRotation(float duration, float startAngle, float endAngle)
    {
        Debug.Log("Swing Started!");
        StartCoroutine(SwingRoutine(duration, startAngle, endAngle));
    }

    private IEnumerator SwingRoutine(float duration, float startAngle, float endAngle)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float angle = Mathf.Lerp(startAngle, endAngle, t);
            transform.localRotation = Quaternion.Euler(0, 0, angle);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localRotation = Quaternion.Euler(0, 0, endAngle);
    }

    #endregion
}

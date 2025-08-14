using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public TextMeshProUGUI levelText;
    public Image hpBarFill;
    public Image mpBarFill;
    public Image staminaBarFill;
    public Image brightnessBarFill;


    private PlayerController player;
    [SerializeField] private PlayerSightController sight;

    #region Skill Cooldown

    public Image eSkillCooldownImage;
    private string eSkillId;
    private float eSkillCooldown = 0f;

    #endregion

    public void Init(PlayerController playerController)
    {
        player = playerController;
        eSkillId = player.GetEquippedSkillId(ESkillSlotType.E);
        var skill = SkillRepository.Get(eSkillId);
        if (skill != null)
            eSkillCooldown = skill.attackCooldown;

        sight = player.GetComponent<PlayerSightController>();
    }

    private void Update()
    {
        if (player == null || string.IsNullOrEmpty(eSkillId))
            return;

        var skill = SkillRepository.Get(eSkillId);
        float lastUseTime = player.GetLastSkillUseTime(eSkillId);
        float cooldown = skill.attackCooldown;

        float elapsed = Time.time - lastUseTime;
        float fill = Mathf.Clamp01(elapsed / cooldown);

        if (eSkillCooldownImage != null)
            eSkillCooldownImage.fillAmount = fill;

        // 레벨 표시
        levelText.text = $"{player.Stat.Level}";

        // 체력바, 마나바, 스태미나바 fillAmount 연동
        hpBarFill.fillAmount = player.Stat.CurrentHP / player.Stat.MaxHP;
        mpBarFill.fillAmount = player.Stat.CurrentMP / player.Stat.MaxMP;
        staminaBarFill.fillAmount = player.Stat.CurrentStamina / player.Stat.MaxStamina;

        if (sight != null && brightnessBarFill != null)
        {
            float value = sight.Brightness;
            float max = 100f;
            // 혹시 maxBrightness 값이 바뀌는 경우:
            if (sight != null)
                max = sight.MaxBrightness;
            float brightnessFill = (max > 0) ? Mathf.Clamp01(value / max) : 0f;
            brightnessBarFill.fillAmount = brightnessFill;

            // 시야 OFF일 때 흐리게 처리 등 UX 향상
            brightnessBarFill.color = sight.IsSightOn
                ? Color.white
                : new Color(1f, 1f, 0.5f, 0.3f);
        }
    }
}

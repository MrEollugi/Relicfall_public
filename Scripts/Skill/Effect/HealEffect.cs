using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealEffect : ISkillEffect
{
    public void Apply(ICombatEntity caster, ICombatEntity target, SkillExecutionContext context)
    {
        var player = (caster as PlayerController) ?? (target as PlayerController);
        if (player == null)
        {
            Debug.LogWarning("[HealEffect] 적용 대상이 없음");
            return;
        }

        var skill = context.Skill;

        if (!string.IsNullOrEmpty(skill.effectPath))
        {
            // 예: skill.effectPath = "Effects/HealVFX"
            GameObject healPrefab = Resources.Load<GameObject>(skill.effectPath);
            if (healPrefab != null)
            {
                // Instantiate at player position
                GameObject fx = GameObject.Instantiate(healPrefab, player.transform.position, Quaternion.identity);
                // 플레이어를 부모로 지정
                fx.transform.SetParent(player.transform);
                fx.transform.localScale = Vector3.one * 0.2f;
                // (선택) 1초 뒤 자동 삭제 등
                GameObject.Destroy(fx, 10f);
            }
        }

        if (!string.IsNullOrEmpty(skill.soundPath))
        {
            AudioClip clip = Resources.Load<AudioClip>(skill.soundPath);
            if (clip != null)
            {
                SoundManager.Instance.PlaySFXAtPosition(
                    clip,
                    player.transform.position,
                    volume: 1f,
                    maxDistance: 16f,
                    pitch: 0f
                );
            }
            else
            {
                Debug.LogWarning($"[HealEffect] Sound not found: {skill.soundPath}");
            }
        }

        switch (skill.healTypeStr)
        {
            case "Instant":
                // Instant heal
                float instantHeal = context.Value > 0
                    ? context.Value
                    : skill.healParam * player.Stat.MaxHP;
                player.Heal(instantHeal);
                break;

            case "Regen":
                // DoT Heal
                float regenHeal = context.Value > 0
                    ? context.Value
                    : skill.healParam * player.Stat.MaxHP;
                player.StartHealRoutine(
                    HealOverTime(player, skill.healParam, skill.healDuration, skill.healTickInterval)
                );
                break;

            case "Leech":
                // Leech
                if (context.CustomData != null && context.CustomData.TryGetValue("damageDealt", out var dmgObj) && dmgObj is float dmg)
                {
                    float leech = context.Value > 0
                        ? context.Value
                        : dmg * skill.healParam;
                    player.Heal(leech);
                }
                break;

            default:
                Debug.LogWarning("[HealEffect] 지원하지 않는 healType: " + skill.healTypeStr);
                break;
        }
    }

    private IEnumerator HealOverTime(PlayerController player, float healRate, float duration, float tickInterval)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float healAmount = healRate * player.Stat.MaxHP;
            player.Heal(healAmount);
            yield return new WaitForSeconds(tickInterval);
            elapsed += tickInterval;
        }

        player.StopHealRoutine();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletEffect : ISkillEffect
{
    public void Apply(ICombatEntity caster, ICombatEntity unused, SkillExecutionContext context)
    {
        var skill = context.Skill;
        int count = skill.projectileCount;
        float spread = skill.spreadAngle;
        float speed = skill.projectileSpeed;

        for (int i = 0; i < count; i++)
        {
            // ÆÛÁü°¢ °è»ê(¼¦°ÇÀÌ¸é spread È°¿ë, ±ÇÃÑ/DMRÀÌ¸é 0)
            float angleOffset = (count == 1) ? 0 : Mathf.Lerp(-spread / 2, spread / 2, (float)i / (count - 1));
            Quaternion rot = caster.Transform.rotation * Quaternion.Euler(0, angleOffset, 0);
            Vector3 dir = rot * Vector3.forward;

            string prefabKey = skill.projectilePrefabName;
            if (!ProjectilePrefabRegistry.PrefabMap.TryGetValue(prefabKey, out var prefab) || prefab == null)
            {
                Debug.LogError($"Projectile prefab not found: {prefabKey}");
                continue; // ÀÌ ÅºÀº ½ºÅµ
            }

            // Projectile ¿ÀºêÁ§Æ® »ý¼º
            var projObj = GameObject.Instantiate(prefab, caster.Transform.position + dir, rot);
            var projectile = projObj.GetComponent<BulletProjectile>();
            projectile.Init(caster, dir, speed, skill.range, skill.damageParam);
        }
    }
}

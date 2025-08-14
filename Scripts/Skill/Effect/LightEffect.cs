using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightEffect : ISkillEffect
{
    public void Apply(ICombatEntity caster, ICombatEntity target, SkillExecutionContext context)
    {
        var player = (caster as PlayerController) ?? (target as PlayerController);
        if (player == null) return;

        float value = context.Value > 0 ? context.Value : context.Skill.lightEffectValue;

        var sight = player.GetComponent<PlayerSightController>();
        if (sight != null)
        {
            sight.AddBrightness(value);
            // TODO: Effect/Sound
        }
    }
}

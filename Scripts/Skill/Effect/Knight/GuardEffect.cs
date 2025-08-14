using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardEffect : ISkillEffect
{
    public void Apply(ICombatEntity caster, ICombatEntity _, SkillExecutionContext context)
    {
        var player = caster as PlayerController;
        if (player == null || context.Skill == null)
            return;

        // 1. Guard Enable
        player.IsGuarding = true;
        player.ApplyMovementModifier(context.Skill.guardMoveSpeedWhileGuard);

        // 2. etc - UI, Effect
        // TODO: Guard UI, Guard Animation
    }
}

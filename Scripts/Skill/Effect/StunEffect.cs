using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunEffect : ISkillEffect
{
    public void Apply(ICombatEntity caster, ICombatEntity target, SkillExecutionContext context)
    {
        // 우선은 공통 필드 사용
        float duration = context.Skill.stunDuration;
        // 필요하면 context.CustomData, context.ComboIndex 등도 사용 가능!
        target.ApplyStun(duration);
    }
}

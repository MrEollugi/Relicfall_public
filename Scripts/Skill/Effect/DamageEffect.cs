using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageEffect : ISkillEffect
{
    public void Apply(ICombatEntity caster, ICombatEntity target, SkillExecutionContext context)
    {
        // ex) Player Attack × damageParam
        float atk = caster.GetAttackPower();
        float dmg = atk * context.Skill.damageParam;
        target.TakeDamage(dmg);

        // Stagger(경직) 누적
        float stagger = context.Skill.staggerValue;
        target.ApplyStagger(stagger);   // ICombatEntity에 ApplyStagger(float) 구현 필요
    }
}

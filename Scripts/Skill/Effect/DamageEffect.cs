using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageEffect : ISkillEffect
{
    public void Apply(ICombatEntity caster, ICombatEntity target, SkillExecutionContext context)
    {
        // ex) Player Attack �� damageParam
        float atk = caster.GetAttackPower();
        float dmg = atk * context.Skill.damageParam;
        target.TakeDamage(dmg);

        // Stagger(����) ����
        float stagger = context.Skill.staggerValue;
        target.ApplyStagger(stagger);   // ICombatEntity�� ApplyStagger(float) ���� �ʿ�
    }
}

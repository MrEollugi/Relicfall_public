using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BleedEffect : ISkillEffect
{
    public void Apply(ICombatEntity caster, ICombatEntity target, SkillExecutionContext context)
    {
        float bleedAmount = context.Skill.bleedBuildUp;

        // target�� Bleed ���� ���� (ICombatEntity�� ApplyBleed �޼��� �߰� �ʿ�)
        target.ApplyBleed(bleedAmount); // ����, ��ü ������ ������Ʈ ������ �°�!
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BleedEffect : ISkillEffect
{
    public void Apply(ICombatEntity caster, ICombatEntity target, SkillExecutionContext context)
    {
        float bleedAmount = context.Skill.bleedBuildUp;

        // target에 Bleed 상태 적용 (ICombatEntity에 ApplyBleed 메서드 추가 필요)
        target.ApplyBleed(bleedAmount); // 예시, 구체 구현은 프로젝트 구조에 맞게!
    }
}

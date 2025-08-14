using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISkillEffect
{
    void Apply(ICombatEntity caster, ICombatEntity target,SkillExecutionContext context);
}

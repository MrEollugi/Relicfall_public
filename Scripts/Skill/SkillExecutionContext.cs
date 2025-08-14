using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillExecutionContext
{
    public ICombatEntity Caster { get; set; }
    public ICombatEntity Target { get; set; }
    public SkillData Skill { get; set; }
    public Dictionary<string, object> CustomData { get; set; } = new();

    public float Value { get; set; }

    // 상황에 따라 추가 확장: 타겟 포지션, 콤보 인덱스, etc
    public Vector3? TargetPosition { get; set; }
    public int? ComboIndex { get; set; }
}

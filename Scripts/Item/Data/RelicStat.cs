using System;
using System.Collections.Generic;
using UnityEngine;

public static class RelicStat
{
    // 스탯 증가용
    private static readonly Dictionary<StatType, Action<PlayerStat, float>> ApplyActions
        = new Dictionary<StatType, Action<PlayerStat, float>>
    {
        { StatType.MaxHP,       (stat, v) => stat.SetMaxHp(stat.MaxHP + v) },
        { StatType.MaxMP,       (stat, v) => stat.SetMaxMp(stat.MaxMP + v) },
        { StatType.MaxStamina,  (stat, v) => stat.SetMaxStamina(stat.MaxStamina + v) },
        { StatType.Attack,      (stat, v) => stat.SetAttack((int)(stat.Attack + v)) },
        { StatType.Defense,     (stat, v) => stat.SetDefense((int)(stat.Defense + v)) },
        { StatType.AttackSpeed, (stat, v) => stat.SetAttackSpeed(stat.AttackSpeed + v) },
        { StatType.CritChance,  (stat, v) => stat.SetCritChance(stat.CritChance + v) },
        { StatType.CritDamage,  (stat, v) => stat.SetCritDamage(stat.CritDamage + v) },
        { StatType.MoveSpeed,   (stat, v) => stat.SetCurrentMoveSpeed(stat.CurrentMovementSpeed + v) },
    };

    // 스탯 감소용
    private static readonly Dictionary<StatType, Action<PlayerStat, float>> RemoveActions
        = new Dictionary<StatType, Action<PlayerStat, float>>
    {
        { StatType.MaxHP,       (stat, v) => stat.SetMaxHp(stat.MaxHP - v) },
        { StatType.MaxMP,       (stat, v) => stat.SetMaxMp(stat.MaxMP - v) },
        { StatType.MaxStamina,  (stat, v) => stat.SetMaxStamina(stat.MaxStamina - v) },
        { StatType.Attack,      (stat, v) => stat.SetAttack((int)(stat.Attack - v)) },
        { StatType.Defense,     (stat, v) => stat.SetDefense((int)(stat.Defense - v)) },
        { StatType.AttackSpeed, (stat, v) => stat.SetAttackSpeed(stat.AttackSpeed - v) },
        { StatType.CritChance,  (stat, v) => stat.SetCritChance(stat.CritChance - v) },
        { StatType.CritDamage,  (stat, v) => stat.SetCritDamage(stat.CritDamage - v) },
        { StatType.MoveSpeed,   (stat, v) => stat.SetCurrentMoveSpeed(stat.CurrentMovementSpeed - v) },
    };

    // 스탯 적용
    public static void ApplyStat(ItemData relicData, PlayerStat stat)
    {
        if (relicData == null || relicData?.statModifiers == null) return;
        foreach (var mod in relicData.statModifiers)
        {
            if (!Enum.TryParse<StatType>(mod.statType, out var statType))
                continue;
            if (ApplyActions.TryGetValue(statType, out var action))
                action(stat, mod.value);
        }
    }

    // 스탯 해제
    public static void RemoveStat(ItemData relicData, PlayerStat stat)
    {
        if (relicData == null || relicData?.statModifiers == null) return;
        foreach (var mod in relicData.statModifiers)
        {
            if (!Enum.TryParse<StatType>(mod.statType, out var statType))
                continue;
            if (RemoveActions.TryGetValue(statType, out var action))
                action(stat, mod.value);
        }
    }
}
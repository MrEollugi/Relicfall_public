using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region Enums
public enum ItemType
{
    Consumable,
    Relic,
    Curios
}

public enum ConsumableType
{
    Health,
    Mana,
    Stamina,
    Torch
}

public enum StatType
{
    None,
    MaxHP,
    MaxMP,
    MaxStamina,
    Attack,
    Defense,
    AttackSpeed,
    CritChance,
    CritDamage,
    MoveSpeed
}
#endregion

[System.Serializable]
public class ItemData
{
    public string id;
    public string name;
    public string type;
    public string iconPath;
    public string dropPrefabAddress;
    public int weight;
    public int value;
    public int valueRange;
    public int sizeX;
    public int sizeY;
    public bool isStackable;
    public int maxStackCount;

    // Consumable 전용
    public string consumableType;
    public float effectValue;

    // Relic 전용
    public List<StatModifier> statModifiers;

    public string skillId;          // 소모품/장비 등에서 사용할 스킬 효과
}

[System.Serializable]
public class StatModifier
{
    public string statType;
    public float value;
}
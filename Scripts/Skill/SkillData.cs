using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region Enums
public enum ETargetType
{
    None,
    Self,
    MousePoint,
    Direction
}

public enum ESkillShape
{
    None = 0,
    Circle = 1,
    Cone = 2,
    Rectangle = 3,
    Line = 4,
}

public enum EMoveType
{
    None,
    Dash,
    Teleport,

}

public enum EAttackType
{
    None,
    Shotgun,
    Handgun,
    DMR,
    AR,
}

public enum EHealType
{
    None,
    Instant,
    Regen,
    Leech
}

public enum ESkillSlotType
{
    BasicAttack = 0,
    RightClick = 1,
    Q = 2,
    E = 3,
    Passive = 4,
    Ultimate = 5,
    Interact = 6,
    UseItem1 = 7,
    UseItem2 = 8,
}
#endregion

[System.Serializable]
public class SkillData
{
    public string id;
    public string name;
    public List<string> effects;

    [System.NonSerialized] public ETargetType targetType;
    public string targetTypeStr;
    [System.NonSerialized] public ESkillShape skillShape;
    public string skillShapeStr;
    public float range;
    public float angleOrWidth;
    public float aoeRadius;
     
    #region Guard
    [Header("Guard Angle")]
    public float guardAngle;

    [Header("Damage Reduction")]
    public float guardReduce;

    [Header("Stamina Cost")]
    public float guardStaminaCostPerStagger;
    public float guardStaminaCostMin;
    public float guardStaminaCostMax;

    [Header("Guard Speed Reduce")]
    public float guardMoveSpeedWhileGuard;
    public float guardMoveSpeedWhenHit;

    [Header("Guard Break Time")]
    public float guardBreakTime;
    
    [Header("Perfact Guard")]
    public float guardPerfectTime;                      // base 0.2s
    public float guardPerfectDMGReduce;                 // use guardReduce data when this field is null
    public float guardPerfectStaminaCostPerStagger;     // use guardStaminaCostPerStagger data when this field is null
    #endregion

    [Header("Movement Skill")]
    [System.NonSerialized] public EMoveType moveType;
    public string moveTypeStr;
    public float moveDistance;
    public float moveSpeed;

    [Header("Projectile")]
    [System.NonSerialized] public EAttackType attackType;
    public string attackTypeStr;           // "shotgun", "bullet", "dmr" 등
    public int projectileCount;         // 탄환 개수 (샷건, 샷건투사체 등)
    public float spreadAngle;           // 샷건 퍼짐 각도
    public string projectilePrefabName; // 투사체 프리팹 Addressable 이름 또는 리소스 키
    public float projectileSpeed;
    public string soundPath;
    [NonSerialized] public AudioClip soundClip; // 런타임 로드 후 캐시
    [System.NonSerialized] public GameObject effectPrefab;
    public string effectPath;



    #region Attack
    [Header("Damage")]
    public float damageParam;

    [Header("Stagger")]
    public float staggerValue;

    public float attackCooldown;
    #endregion

    public float staminaCost;
    public float manaCost;

    #region Heal
    [Header("Heal Type")]
    [System.NonSerialized] public EHealType healType;
    public string healTypeStr;

    [Header("Heal")]
    public float healParam;
    public float healDuration;
    public float healTickInterval;
    #endregion

    #region

    public float lightCostValue;
    public float lightEffectValue;

    #endregion

    #region Status Effect
    public float stunChance;
    public float stunDuration;

    public float knockbackChance;
    public float knockbackForce;

    public float bleedBuildUp;
    #endregion


    public Dictionary<string, float> customParams;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] 
public class EnemyStat : IStat
{
    #region Resources
    private int level;

    [Header("Resources")]
    [SerializeField] private float maxHP;
    [SerializeField] private float maxMP;

    [Header("MP")]
    private float currentHP;
    private float currentMP;

    [Header("Stamina")]
    [SerializeField] private float maxStamina;
    private float currentStamina;
    #endregion

    #region Movement

    [Header("Movement Speed")]
    [SerializeField] private float maxMovementSpeed;
    [SerializeField] private float movementSpeed;
    private float currentMoveSpeed;

    [Header("Movement Modifier")]
    private float currentRunMultiplier;
    private float currentSneakMultiplier;

    #endregion

    #region Battle Stats
    [Header("Attack & Defense")]
    [SerializeField] private int attack;
    [SerializeField] private float attackSpeed;
    [SerializeField] private int defense;

    [Header("Critical")]
    [SerializeField] private float critChance;
    [SerializeField] private float critDamage;

    [Header("Dodge")]
    [SerializeField] private float dodgeRate;
    #endregion

    #region EStatusEffect Resists
    [Header("Status Effect Resist")]
    [SerializeField] private float poisonResist;
    [SerializeField] private float bleedResist;
    [SerializeField] private float fireResist;
    [SerializeField] private float stunResist;
    [SerializeField] private float fearResist;
    [SerializeField] private float madnessResist;
    #endregion

    #region Stagger

    [SerializeField] private int maxStaggerValue;
    private int currentStaggerResValue;

    #endregion

    #region Public getter

    public int Level => level;

    #region Resources
    public float MaxHP => maxHP;
    public float MaxMP => maxMP;
    public float MaxStamina => maxStamina;
    public float CurrentHP => currentHP;
    public float CurrentMP => currentMP;
    public float CurrentStamina => currentStamina;
    #endregion

    #region Movement

    public float MaxMovementSpeed => maxMovementSpeed;
    public float MovementSpeed => movementSpeed;
    public float CurrentMovementSpeed => currentMoveSpeed;
    public float CurrentRunSpeedMultiplier => currentRunMultiplier;
    public float CurrentSneakSpeedMultiplier => currentSneakMultiplier;

    #endregion

    #region Attack & Defense
    public float Attack => attack;
    public float AttackSpeed => attackSpeed;
    public float Defense => defense;
    #endregion

    #region Critical
    public float CritChance => critChance;
    public float CritDamage => critDamage;
    #endregion

    #region Status Effect Resist
    public float PoisonResist => poisonResist;
    public float BleedResist => bleedResist;
    public float FireResist => fireResist;
    public float StunResist => stunResist;
    public float FearResist => fearResist;
    public float MadnessResist => madnessResist;
    #endregion

    #region Stagger

    public float MaxStarggerResValue => maxStaggerValue;
    public float CurrentStaggerResValue => currentStaggerResValue;

    #endregion

    #endregion

    public void Initialize(EnemyDataSO enemy)
    {
        #region Init Resources
        maxHP = enemy.baseHP;
        currentHP = maxHP;

        maxMP = enemy.baseMP;
        currentMP = maxMP;

        maxStamina = enemy.baseStamina;
        currentStamina = maxStamina;
        #endregion


        movementSpeed = enemy.baseMoveSpeed;
        currentMoveSpeed = movementSpeed;
        currentRunMultiplier = enemy.baseRunMultiplier;
        currentSneakMultiplier = enemy.baseSneakMultiplier;
    }

    public void SetCurrentHP(float hp) => currentHP = Mathf.Clamp(hp, 0, maxHP);
    public void SetCurrentMP(float mp) => currentMP = Mathf.Clamp(mp, 0, maxMP);
    public void SetCurrentStamina(float st) => currentStamina = Mathf.Clamp(st, 0, maxStamina);

    public void UseStamina(float amount)
    {
        currentStamina -= amount;
    }

    public void TakeDamage(float damage)
    {
        currentHP = Mathf.Max(0, currentHP - damage);
    }

    public bool IsDead() => currentHP <= 0;
}

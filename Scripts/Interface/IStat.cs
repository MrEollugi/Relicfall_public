using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStat
{
    #region Resources
    int Level { get; }

    float MaxHP { get; }
    float CurrentHP { get; }

    float MaxMP { get; }
    float CurrentMP { get; }

    float MaxStamina { get; }
    float CurrentStamina { get; }

    #endregion

    #region Movement

    float MaxMovementSpeed { get; }
    float MovementSpeed { get; }
    float CurrentMovementSpeed { get; }
    float CurrentRunSpeedMultiplier { get; }
    float CurrentSneakSpeedMultiplier {  get; }

    #endregion

    #region Battle Stat
    float Attack { get; }
    float Defense { get; }
    float AttackSpeed { get; }
    #endregion

    #region Critical
    float CritChance { get; }
    float CritDamage { get; }
    #endregion

    #region Status Effect Resist
    float PoisonResist {  get; }
    float BleedResist {  get; }
    float FireResist {  get; }
    float StunResist { get; }
    float FearResist { get; }
    float MadnessResist {  get; }
    #endregion

    #region Stagger

    float MaxStarggerResValue {  get; }
    float CurrentStaggerResValue { get; }

    #endregion

    void SetCurrentHP(float hp);
    void SetCurrentMP(float mp);
    void SetCurrentStamina(float st);
    void UseStamina(float amount);
    void TakeDamage(float damage);
    bool IsDead();
}

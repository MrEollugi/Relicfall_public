using UnityEngine;

public enum EnemyType
{
    Normal,
    Boss,
}

[CreateAssetMenu(menuName = "Enemy/EnemyData")]
public class EnemyDataSO : ScriptableObject
{
    public string enemyName;
    public EnemyType type;

    [Header("Movement")]
    public float baseMoveSpeed;
    public float baseRunMultiplier;
    public float baseSneakMultiplier;

    [Header("Resources")]
    public float baseHP;
    public float baseMP;
    public float baseStamina;
    public float baseStaminaRegen;

    [Header("Interaction")]
    public float baseInteractionSpeed;

    [Header("Sight")]
    public float baseSightRange;
    public float baseDarkSightRange;

    [Header("Battle Stat")]
    public float baseAttack;
    public float baseDefense;
    public float baseAttackSpeed;

    [Header("Critical")]
    public float baseCritChance;
    public float baseCritDamage;

    [Header("Dodge")]
    public float baseDodgeChance;

    [Header("Status Effects")]
    public float resPercent_basePoison;
    public float resPercent_baseBleed;
    public float resPercent_baseFire;
    public float resPercent_baseFear;
    public float resPercent_baseMadness;

    public float attackRange = 2f;
    public float maxChaseRange = 15f;
    public float chaseSpeed = 4f;
    public float stalkSpeed = 2f;

    [Header("프리펩")]
    public GameObject enemyPrefabs;

}



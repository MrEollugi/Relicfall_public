using UnityEngine;
using UnityEngine.ParticleSystemJobs;

[System.Serializable]
public class PlayerStat : IStat
{
    private const float MOVE_SPEED_LIMIT = 8f;

    #region Private

    public int level;
    private PlayerJobSO jobData;

    #region Resources



    [Header("HP")]
    [SerializeField] private float maxHP;
    private float regenHP;
    private float currentHP;


    [Header("MP")]
    [SerializeField] private float maxMP;
    private float regenMP;
    private float currentMP;

    [Header ("Stamina")]
    [SerializeField] private float maxStamina;
    private float currentStamina;

    #endregion

    #region Movement

    [Header("Movement Speed")]
    [SerializeField] private float maxMovementSpeed;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float currentMoveSpeed;

    [Header("Movement Modifier")]
    private float currentRunMultiplier;
    private float currentSneakMultiplier;

    #endregion

    #region Attack & Defense

    [Header("Battle Stat")]
    [SerializeField] private float attack;
    [SerializeField] private float defense;
    [SerializeField] private float attackSpeed;

    #endregion

    #region Critical

    [Header("Critical")]
    [SerializeField] private float critChance;
    [SerializeField] private float critDamage;

    #endregion

    #region Dodge

    [Header("Dodge")]
    [SerializeField] private float dodgeChance;

    #endregion

    #region Status Effect Resist
    [Header("Status Effect Resistance")]
    [SerializeField] private float poisonResist;
    [SerializeField] private float bleedResist;
    [SerializeField] private float fireResist;
    [SerializeField] private float stunResist;
    [SerializeField] private float fearResist;
    [SerializeField] private float madnessResist;
    #endregion

    #region Stagger

    [SerializeField] private float maxStaggerValue;
    private float currentStaggerResValue;

    #endregion

    #region Sight
    [Header("Sight")]
    [SerializeField] private float sightRange;
    [SerializeField] private float darkSightRange;
    #endregion

    #endregion

    #region Public getter

    #region Resources
    public float MaxHP => maxHP;
    public float RegenHP => regenHP;
    public float CurrentHP => currentHP;
    public float MaxMP => maxMP;
    public float RegenMP => regenMP;
    public float CurrentMP => currentMP;
    public float MaxStamina => maxStamina;
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
    public float Defense => defense;
    public float AttackSpeed => attackSpeed;
    #endregion

    #region Critical
    public float CritChance => critChance;
    public float CritDamage => critDamage;
    #endregion

    #region Dodge
    public float DodgeChance => dodgeChance;
    #endregion

    #region Public Status Effect Resist
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

    #region Level
    public int Level => level;
    #endregion

    #endregion

    // when start game (1 time)
    // stat.InitializeFromJob(jobSO): so에 들어있는 레벨별 스탯 테이블을 읽어서 값을 세팅
    public void InitializeFromJob(PlayerJobSO jobSO)
    {
        jobData = jobSO;

        SetLevel(1);

        currentRunMultiplier = jobData.baseRunMultiplier;
        currentSneakMultiplier = jobData.baseSneakMultiplier;
    }

    #region Set Current

    #region Resource
    public void SetCurrentHP(float hp) => currentHP = Mathf.Clamp(hp, 0, maxHP);
    public void SetCurrentMP(float mp) => currentMP = Mathf.Clamp(mp, 0, maxMP);
    public void SetCurrentStamina(float st) => currentStamina = Mathf.Clamp(st, 0, maxStamina);

    public void SetMaxHp(float hp)
    {
        float oldMaxHP = maxHP;

        maxHP = Mathf.Max(1, hp);

        float maxHpChange = maxHP - oldMaxHP;

        SetCurrentHP(currentHP + maxHpChange);
    }
    public void SetMaxMp(float mp)
    {
        float oldMaxMp = maxMP;

        maxMP = Mathf.Max(1, mp);

        float newCurrentMP = maxMP - oldMaxMp;

        SetCurrentHP(newCurrentMP);
    }
    public void SetMaxStamina(float stamina)
    {
        float oldMaxStamina = maxStamina;

        maxStamina = Mathf.Max(1, stamina);

        float newCurrentStamina = maxStamina - oldMaxStamina;

        SetCurrentHP(newCurrentStamina);
    }
    #endregion

    #region Movement
    public void SetCurrentMoveSpeed(float spd) => currentMoveSpeed = Mathf.Clamp(spd, 0, maxMovementSpeed);
    public void SetRunMultiplier(float m) => currentRunMultiplier = m;
    public void SetSneakMultiplier(float m) => currentSneakMultiplier = m;
    #endregion

    #region Battle
    public void SetAttack(int atk) => attack = atk;
    public void SetDefense(int def) => defense = def;
    public void SetAttackSpeed(float spd) => attackSpeed = spd;
    #endregion

    #region Crit
    public void SetCritChance(float c) => critChance = c;
    public void SetCritDamage(float d) => critDamage = d;
    #endregion

    #region Dodge
    public void SetDodgeChance(float d) => dodgeChance = d;
    #endregion

    #region Status Resist
    public void SetPoisonResist(float v) => poisonResist = v;
    public void SetBleedResist(float v) => bleedResist = v;
    public void SetFireResist(float v) => fireResist = v;
    public void SetStunResist(float v) => stunResist = v;
    public void SetFearResist(float v) => fearResist = v;
    public void SetMadnessResist(float v) => madnessResist = v;
    #endregion

    #region Stagger
    public void SetCurrentStaggerResValue(float v) => currentStaggerResValue = v;
    #endregion

    #region Sight
    public void SetSightRange(float r) => sightRange = r;
    public void SetDarkSightRange(float r) => darkSightRange = r;
    #endregion

    #region Level
    // if needed to adjust on-the-fly
    public void SetLevel(int newLevel)
    {
        level = Mathf.Clamp(newLevel, 1, jobData.maxLevel);
        
        maxHP = jobData.hpCurve.Evaluate(level);
        currentHP = Mathf.Min(currentHP, maxHP);

        // If you have separate tables for MP/Stamina, replace these with appropriate arrays
        maxMP = jobData.mpCurve.Evaluate(level);
        currentMP = Mathf.Min(currentMP, maxMP);

        maxStamina = jobData.staminaCurve.Evaluate(level);
        currentStamina = Mathf.Min(currentStamina, maxStamina);

        maxMovementSpeed = jobData.moveSpeedCurve.Evaluate(level);
        currentMoveSpeed = maxMovementSpeed;

        attack = jobData.attackCurve.Evaluate(level);
        defense = jobData.defenseCurve.Evaluate(level);
        attackSpeed = jobData.attackSpeedCurve.Evaluate(level);

        critChance = jobData.critChanceCurve.Evaluate(level);
        critDamage = jobData.critDamageCurve.Evaluate(level);

        dodgeChance = jobData.dodgeChanceCurve.Evaluate(level);
    }
    #endregion

    #endregion

    // Resource regen
    public void Regenerate(float deltaTime)
    {
        float staminaRegenPerSecond = maxStamina * 0.1f;
        currentStamina = Mathf.Min(maxStamina, currentStamina + staminaRegenPerSecond * deltaTime);

        currentHP = Mathf.Min(maxHP, currentHP + regenHP * deltaTime);
        currentMP = Mathf.Min(maxMP, currentMP + regenMP * deltaTime);
    }
    public void Heal(float amount)
    {
        currentHP = Mathf.Min(maxHP, currentHP + amount);
    }
    public void TakeDamage(float damage)
    {
        currentHP = Mathf.Max(0, currentHP - damage);
    }
    public void UseStamina(float amount)
    {
        currentStamina -= amount;
    }
    public bool IsDead()
    {
        return currentHP <= 0;
    }


    public void ApplyAttackSpeedBuff(float multiplier)
    {
        attackSpeed *= multiplier;
    }
}

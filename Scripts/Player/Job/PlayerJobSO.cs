using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

// PlayerJobSO Addressable
[CreateAssetMenu(menuName = "Player/JobData")]
public class PlayerJobSO : ScriptableObject
{
    public string JobId;
    public AssetReference JobAsset;
    public GameObject DeadPrefab;


    [Header("Level > HP Cuve")]
    public AnimationCurve hpCurve = AnimationCurve.Linear(1f, 100f, 10f, 200f);
    [Header("Level → MP Curve")]
    public AnimationCurve mpCurve = AnimationCurve.Linear(1f, 50f, 10f, 100f);
    [Header("Level → Stamina Curve")]
    public AnimationCurve staminaCurve = AnimationCurve.Linear(1f, 30f, 10f, 60f);
    public float baseStaminaRegen;

    [Header("Movement")]
    public AnimationCurve moveSpeedCurve = AnimationCurve.Linear(1f, 3.5f, 10f, 3.5f);
    public float baseRunMultiplier = 1.3f;
    public float baseSneakMultiplier = 0.5f;

    [Header("Battle")]
    public AnimationCurve attackCurve = AnimationCurve.Linear(1f, 10f, 10f, 20f);
    public AnimationCurve defenseCurve = AnimationCurve.Linear(1f, 5f, 10f, 25f);
    public AnimationCurve attackSpeedCurve = AnimationCurve.Linear(1f, 1f, 10f, 2f);
    public AnimationCurve critChanceCurve = AnimationCurve.Linear(1f, 0f, 10f, 0.1f);
    public AnimationCurve critDamageCurve = AnimationCurve.Linear(1f, 1.5f, 10f, 1.5f);
    public AnimationCurve dodgeChanceCurve = AnimationCurve.Linear(1f, 0f, 10f, 10f);

    [Header("Resist")]
    public AnimationCurve poisonResCurve = AnimationCurve.Linear(1f, 100f, 10f, 200f);
    public AnimationCurve bleedResCurve = AnimationCurve.Linear(1f, 100f, 10f, 200f);
    public AnimationCurve fireResCurve = AnimationCurve.Linear(1f, 100f, 10f, 200f);
    public AnimationCurve fearResCurve = AnimationCurve.Linear(1f, 100f, 10f, 200f);
    public AnimationCurve madnessResCurve = AnimationCurve.Linear(1f, 100f, 10f, 200f);

    public int maxLevel = 10;

    public List<ISkillBehavior> Skills;

    [Header("Interaction")]
    public AnimationCurve interactSpeedCurve = AnimationCurve.Linear(1f, 1f, 10f, 1.5f);

    [Header("Sight")]
    public float baseSightRange;
    public float baseDarkSightRange;

    [Header("Stagger")]
    public float baseStaggerResValue;

}


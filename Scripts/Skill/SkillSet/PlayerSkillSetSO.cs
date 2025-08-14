using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player/PlayerSkillSet")]
public class PlayerSkillSetSO : ScriptableObject
{
    [Header("Basic Attack")]
    public string equippedBasicAttackSkillId;

    [Header("Right Click")]
    public List<string> rightClickSkillIds;
    public string equippedRightClickSkillId;

    [Header("Q")]
    public List<string> qSkillIds;
    public string equippedQSkillId;

    [Header("E")]
    public List<string> eSkillIds;
    public string equippedESkillId;

    [Header("Ultimate")]
    public List<string> ultimateSkillIds;
    public string equippedUltimateSkillId;

    [Header("Timed Items")]
    public string equippedUseItem1SkillId;
    public string equippedUseItem2SkillId;
}

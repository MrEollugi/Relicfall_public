using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BehaviorTree/Decorator/DamagedDetected")]
public class DamagedDetectedConditionSO : ConditionSO
{
    [Header("피격 시 효과 지속시간")]
    public float duration = 2f;

    public override bool Check(Blackboard blackboard)
    {
        return Time.time - blackboard.LastHitTime <= duration;
    }
}

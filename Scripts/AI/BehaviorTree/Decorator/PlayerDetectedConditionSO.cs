using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BehaviorTree/Decorator/PlayerDetected")]
public class PlayerDetectedConditionSO : ConditionSO
{
    [Tooltip("감지 반경")]
    public float detectRadius = 10f;

    public override bool Check(Blackboard blackboard)
    {
        if (blackboard.Player == null)
        {
            Debug.LogWarning("[PlayerDetectedConditionSO] Player is null!");
            return false;
        }

        return blackboard.IsPlayerVisible || blackboard.isPlayerIn;
    }
}

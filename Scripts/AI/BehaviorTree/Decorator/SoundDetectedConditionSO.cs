using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

[CreateAssetMenu(menuName = "BehaviorTree/Decorator/SoundDetected")]
public class SoundDetectedConditionSO : ConditionSO
{
    [Tooltip("소리 감지 반경")]
    public float detectRadius = 10f;

    public override bool Check(Blackboard blackboard)
    {
        if (blackboard.Player == null)
        {
            Debug.LogWarning("[SoundDetectedConditionSO] Player is null!");
            return false;
        }

        if(blackboard.transform == null)
        {
            Debug.Log("[SoundDetectedConditionSO] transform is null!");
            return false;
        }

        if (blackboard.lastHeardPosition == UnityEngine.Vector3.zero)
            return false;


        
        float distance = UnityEngine.Vector3.Distance(blackboard.transform.position, blackboard.lastHeardPosition);

        if(distance <= detectRadius)
            return true;
        else
            return false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "BehaviorTree/Action/FollowPlayer")]
public class FollowPlayerActionSO : BTNodeSO
{
    [Tooltip("쫓아갈 속도 계수")]
    public float speedMultiplier = 1f;

    public override BTNode Build()
    {
        return new FollowPlayerAction(speedMultiplier);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "BehaviorTree/Action/FollowPlayer")]
public class FollowPlayerActionSO : BTNodeSO
{
    [Tooltip("�Ѿư� �ӵ� ���")]
    public float speedMultiplier = 1f;

    public override BTNode Build()
    {
        return new FollowPlayerAction(speedMultiplier);
    }
}

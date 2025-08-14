using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BehaviorTree/Action/Idle")]
public class IdleActionSO : BTNodeSO
{
    public override BTNode Build()
        => new IdleAction();
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BehaviorTree/Decorator/Condition")]
public class ConditionDecoratorNodeSO : BTNodeSO
{
    [Tooltip("검사할 ConditionSO")]
    public ConditionSO condition;
    [Tooltip("조건이 true일 때 실행할 자식 노드")]
    public BTNodeSO child;

    public override BTNode Build()
    {
        return new ConditionDecoratorNode(condition, child.Build());
    }
}

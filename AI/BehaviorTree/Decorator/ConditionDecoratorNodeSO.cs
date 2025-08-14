using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BehaviorTree/Decorator/Condition")]
public class ConditionDecoratorNodeSO : BTNodeSO
{
    [Tooltip("�˻��� ConditionSO")]
    public ConditionSO condition;
    [Tooltip("������ true�� �� ������ �ڽ� ���")]
    public BTNodeSO child;

    public override BTNode Build()
    {
        return new ConditionDecoratorNode(condition, child.Build());
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BehaviorTree/Decorator/Inverter")]
public class InverterDecoratorNodeSO : BTNodeSO
{
    [Tooltip("������ �ڽ� ���")]
    public BTNodeSO child;

    public override BTNode Build()
    {
        return new InverterDecoratorNode(child.Build());
    }
}

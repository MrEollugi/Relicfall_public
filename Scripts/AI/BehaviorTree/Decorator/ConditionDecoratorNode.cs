using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionDecoratorNode : BTNode
{
    private readonly ConditionSO _condition;
    private readonly BTNode _child;

    public ConditionDecoratorNode(ConditionSO condition, BTNode child)
    {
        _condition = condition;
        _child = child;
    }

    public override NodeState Tick(Blackboard blackboard)
    {
        if (!_condition.Check(blackboard))
        { 
            return NodeState.Failure; 
        }
        
        return _child.Tick(blackboard);
    }
}

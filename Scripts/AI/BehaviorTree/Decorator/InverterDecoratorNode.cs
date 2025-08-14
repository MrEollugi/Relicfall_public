using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InverterDecoratorNode : BTNode
{
    private readonly BTNode _child;

    public InverterDecoratorNode(BTNode child)
    {
        _child = child;
    }

    public override NodeState Tick(Blackboard blackboard)
    {
        var result = _child.Tick(blackboard);
        // Success ¡ê Failure, Running À¯Áö
        if (result == NodeState.Success) return NodeState.Failure;
        if (result == NodeState.Failure) return NodeState.Success;
        return NodeState.Running;
    }
}

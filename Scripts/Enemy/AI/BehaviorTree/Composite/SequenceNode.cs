using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequenceNode : BTNode
{
    private readonly List<BTNode> _children;
    private int _currentIndex;

    public SequenceNode(List<BTNode> children)
    {
        _children = children;
        _currentIndex = 0;
    }

    public override NodeState Tick(Blackboard blackboard)
    {
        for(; _currentIndex < _children.Count;)
        {
            NodeState result = _children[_currentIndex].Tick(blackboard);
            if(result == NodeState.Running) return NodeState.Running;
            if(result == NodeState.Failure)
            {
                _currentIndex = 0;
                return NodeState.Failure;
            }
            _currentIndex++;
        }
        _currentIndex = 0;
        return NodeState.Success;
    }
}

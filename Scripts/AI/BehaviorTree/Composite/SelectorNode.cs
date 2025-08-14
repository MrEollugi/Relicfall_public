using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectorNode : BTNode
{
    private readonly List<BTNode> _children;
    private int _currentIndex;

    public SelectorNode(List<BTNode> children)
    {
        _children = children;
        _currentIndex = 0;
    }

    public override NodeState Tick(Blackboard blackboard)
    {
        _currentIndex = 0;

        for (; _currentIndex < _children.Count;)
        {
            var result = _children[_currentIndex].Tick(blackboard);
            if (result == NodeState.Running)
                return NodeState.Running;
            if (result == NodeState.Success)
            {
                _currentIndex = 0;
                return NodeState.Success;
            }
            _currentIndex++;
        }
        _currentIndex = 0;
        return NodeState.Failure;
    }
}

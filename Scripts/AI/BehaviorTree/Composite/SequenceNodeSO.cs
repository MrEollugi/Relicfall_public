using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BehaviorTree/Composite/Sequence")]
public class SequenceNodeSO : BTNodeSO
{
    [Tooltip("from Left")]
    public List<BTNodeSO> children;

    public override BTNode Build()
    {
        var nodes = new List<BTNode>();
        foreach(var child in children)
        {
            nodes.Add(child.Build());
        }
        return new SequenceNode(nodes);
    }
}

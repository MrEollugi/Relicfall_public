using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BehaviorTree/Composite/Selector")]
public class SelectorNodeSO : BTNodeSO
{
    public List<BTNodeSO> children;

    public override BTNode Build()
    {
        List<BTNode> nodes = new List<BTNode>();
        foreach(BTNodeSO child in children)
        {
            nodes.Add(child.Build());
        }
        return new SelectorNode(nodes);
    }
}

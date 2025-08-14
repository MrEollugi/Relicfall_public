using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BehaviorTree/BehaviorTreeAsset")]
public class BehaviorTreeAsset : ScriptableObject
{
    [Tooltip("루트 노드 SO를 할당")]
    public BTNodeSO rootNode;

    public BTNode BuildTree()
    {
        if (rootNode == null)
        {
            Debug.LogError($"[{name}] RootNode가 할당되지 않았습니다.");
            return null;
        }
        return rootNode.Build();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BehaviorTree/BehaviorTreeAsset")]
public class BehaviorTreeAsset : ScriptableObject
{
    [Tooltip("��Ʈ ��� SO�� �Ҵ�")]
    public BTNodeSO rootNode;

    public BTNode BuildTree()
    {
        if (rootNode == null)
        {
            Debug.LogError($"[{name}] RootNode�� �Ҵ���� �ʾҽ��ϴ�.");
            return null;
        }
        return rootNode.Build();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Blackboard))]
public class BehaviorTreeRunner : MonoBehaviour
{
    [Tooltip("이 몬스터가 사용할 BehaviorTreeAsset")]
    [SerializeField] private BehaviorTreeAsset treeAsset;

    private BTNode _rootNode;
    private Blackboard _blackboard;

    void Awake()
    {
        _blackboard = GetComponent<Blackboard>();
        _rootNode = treeAsset?.BuildTree();
        if (_rootNode == null)
            Debug.LogError($"[{name}] BehaviorTreeAsset 또는 루트 노드가 설정되지 않았습니다.");
        var agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    void Update()
    {
        var enemy = GetComponent<EnemyController>();
        if (enemy == null || !enemy.IsAlive || !enemy.Agent.enabled)
            return;

        if (_rootNode != null)
            _rootNode.Tick(_blackboard);
    }
}

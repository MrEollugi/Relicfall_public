using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrolAction : BTNode
{
    private readonly float _radius;
    private NavMeshAgent _agent;
    private Animator _anim;
    private Transform _t;
    private Vector3 _target;
    private bool _init;

    public PatrolAction(float radius) { _radius = radius; }

    void InitIfNeeded(Blackboard bb)
    {
        if (_init) return;
        _init = true;
        _agent = bb.GetComponent<NavMeshAgent>();
        _anim = bb.GetComponentInChildren<Animator>();
        _t = bb.transform;
        PickNewDestination();
    }

    void PickNewDestination()
    {
        if (_agent == null || !_agent.isOnNavMesh)
            return;

        var dir = Random.insideUnitSphere * _radius;
        dir.y = 0;
        var dest = _t.position + dir;
        if (NavMesh.SamplePosition(dest, out var hit, _radius, NavMesh.AllAreas))
            dest = hit.position;
        _target = dest;
        _agent.SetDestination(_target);
    }

    public override NodeState Tick(Blackboard bb)
    {
        InitIfNeeded(bb);

        var enemy = bb.GetComponent<EnemyController>();
        if (enemy == null || !enemy.IsAlive || !enemy.Agent.enabled)
        {
            if (_anim != null) _anim.SetBool("IsMoving", false);
            return NodeState.Failure;
        }

        if (_anim != null) _anim.SetBool("IsMoving", true);

        if (_agent == null || !_agent.isOnNavMesh)
            return NodeState.Failure;

        if (_agent.pathPending || _agent.remainingDistance > 0.5f)
            return NodeState.Running;

        // 목적지에 도달하면 다시 새 목적지 선택
        PickNewDestination();
        return NodeState.Running;
    }
}

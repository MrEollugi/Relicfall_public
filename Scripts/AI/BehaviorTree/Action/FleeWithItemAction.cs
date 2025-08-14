using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FleeWithItemAction : BTNode
{
    private readonly float _speedMult;
    private readonly Vector3 _safePoint;
    private NavMeshAgent _agent;
    private Animator _anim;

    public FleeWithItemAction(float speedMult, Vector3 safePoint)
    {
        _speedMult = speedMult;
        _safePoint = safePoint;
    }

    private void EnsureInit(Blackboard bb)
    {
        if (_agent != null) return;
        _agent = bb.GetComponent<NavMeshAgent>();
        _anim = bb.GetComponent<Animator>();
    }

    public override NodeState Tick(Blackboard bb)
    {
        EnsureInit(bb);

        var enemy = bb.GetComponent<EnemyController>();
        if (enemy == null || !enemy.IsAlive || !enemy.Agent.enabled)
        {
            if (_anim != null) _anim.SetBool("IsMoving", false);
            return NodeState.Failure;
        }

        _agent.speed = bb.GetComponent<EnemyController>().EnemyData.baseMoveSpeed * _speedMult;
        _agent.SetDestination(_safePoint);
        _anim.SetBool("IsMoving", true);

        if (Vector3.Distance(bb.transform.position, _safePoint) < 0.5f)
        {
            // 아이템 탈취 완료 로직
            bb.GetComponent<EnemyController>().OnFleeComplete();
            return NodeState.Success;
        }
        return NodeState.Running;
    }
}

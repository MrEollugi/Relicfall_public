using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class IdleAction : BTNode
{
    private Animator _anim;
    private NavMeshAgent _agent;
    private Transform _t;
    private bool _init;

    void InitIfNeeded(Blackboard bb)
    {
        if (_init) return;
        _init = true;
        _agent = bb.GetComponent<NavMeshAgent>();
        _anim = bb.GetComponentInChildren<Animator>();
        _t = bb.transform;
    }

    public override NodeState Tick(Blackboard blackboard)
    {
        InitIfNeeded(blackboard);

        var enemy = blackboard.GetComponent<EnemyController>();
        if (enemy == null || !enemy.IsAlive || !enemy.Agent.enabled)
        {
            if (_anim != null) _anim.SetBool("IsMoving", false);
            return NodeState.Failure;
        }

        // ¿Ãµø ∏ÿ√ﬂ±‚
        if (_agent != null)
        {
            _agent.SetDestination(_t.position);
            _agent.velocity = Vector3.zero;
        }

        if (_anim != null)
            _anim.SetBool("IsMoving", false);

        return NodeState.Running;
    }
}

using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FollowSound : BTNode
{
    private NavMeshAgent _agent;
    private Transform _transform;
    private Animator _anim;
    private SpriteRenderer _sprite;
    private string _what;

    public FollowSound()
    {
        
    }
    private void EnsureInit(Blackboard bb)
    {
        if (_agent != null) return;
        _agent = bb.GetComponent<NavMeshAgent>();
        _transform = bb.transform;
        _anim = bb.GetComponentInChildren<Animator>();

        if (_sprite == null)
            _sprite = bb.GetComponentInChildren<SpriteRenderer>();
    }

    public override NodeState Tick(Blackboard bb)
    {
        EnsureInit(bb);

        if (bb.lastHeardPosition == Vector3.zero)
            return NodeState.Failure;


        if (_agent.destination != bb.lastHeardPosition)
            _agent.SetDestination(bb.lastHeardPosition);


        if (Vector3.Distance(bb.transform.position, bb.lastHeardPosition) < 0.2f)
            return NodeState.Success;


        return NodeState.Running;
    }
}

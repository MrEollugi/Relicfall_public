using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FollowPlayerAction : BTNode
{
    private readonly float _speedMult;
    private NavMeshAgent _agent;
    private Transform _transform;
    private Animator _anim;
    private SpriteRenderer _sprite;

    public FollowPlayerAction(float speedMultiplier)
    {
        _speedMult = speedMultiplier;
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

        var enemy = bb.GetComponent<EnemyController>();
        if (enemy == null || !enemy.IsAlive || !enemy.Agent.enabled)
        {
            if (_anim != null) _anim.SetBool("IsMoving", false);
            return NodeState.Failure;
        }

        if (bb.Player == null)
        {
            if (_anim != null)
                _anim.SetBool("IsMoving", false);

            return NodeState.Running; // or Failure
        }

        // Move To Player
        Vector3 targetPos;
        if (bb.IsPlayerVisible)
        {
            // 시야에 보이면 무조건 플레이어 위치로 추적
            targetPos = bb.Player.position;
        }
        else if (bb.lastHeardPosition != Vector3.zero)
        {
            // 소리 좌표(마지막 들은 위치)로 이동
            targetPos = bb.lastHeardPosition;
        }
        else
        {
            // 아무것도 없으면 (예외) 자기 위치 유지
            targetPos = _transform.position;
        }

        _agent.speed = bb.GetComponent<EnemyController>().EnemyData.baseMoveSpeed * _speedMult;
        _agent.SetDestination(bb.Player.position);

        if (_anim != null) _anim.SetBool("IsMoving", true);

        // 도착했는지 검사 (예: 0.5m 이내면 Success)
        if (Vector3.Distance(_transform.position, targetPos) < 0.5f)
        {
            if (!bb.IsPlayerVisible && bb.lastHeardPosition != Vector3.zero)
                bb.lastHeardPosition = Vector3.zero;
            return NodeState.Success;
        }

        Vector3 vel = _agent.velocity;
        if (_sprite != null && Mathf.Abs(vel.x) > 0.1f)
            _sprite.flipX = vel.x < 0f;

        return NodeState.Running;
    }
}
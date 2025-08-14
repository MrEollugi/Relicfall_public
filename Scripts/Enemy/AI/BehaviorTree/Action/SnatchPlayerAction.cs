using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnatchPlayerAction : BTNode
{
    private readonly float _range;
    private Transform _player;
    private Animator _anim;

    public SnatchPlayerAction(float range) { _range = range; }

    private void EnsureInit(Blackboard bb)
    {
        if (_player != null) return;
        _player = bb.Player;
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

        // 사정거리 내인가?
        if (Vector3.Distance(bb.transform.position, _player.position) > _range)
            return NodeState.Failure;

        // 제압 애니메이션 & 플레이어 상태 잠금
        _anim.SetTrigger("Snatch");
        bb.GetComponent<EnemyController>().OnSnatchPlayer(_player);

        return NodeState.Success;
    }
}

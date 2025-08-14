using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChargeAction : BTNode//돌진
{
    private readonly float _chargeRange;
    private readonly float _chargeSpeed;
    private readonly float _chargeDuration;
    private readonly float _chargePointRange;
    private readonly float _chargeCoolTime;
    private NavMeshAgent _agent;
    private Transform _transform;
    private Animator _anim;
    private SpriteRenderer _sprite;
    private ChargeAttack attack;
    private bool _chargeStarted = false;

    public ChargeAction(float chargeRange, float chargeSpeed, float chargePointRange, float chargeDuraion, float chargeCoolTime)
    {
        _chargeRange = chargeRange;
        _chargeSpeed = chargeSpeed;
        _chargePointRange = chargePointRange;
        _chargeDuration = chargeDuraion;
        _chargeCoolTime = chargeCoolTime;
    }
    private void EnsureInit(Blackboard bb)
    {
        if (attack == null)
            attack = bb.GetComponent<ChargeAttack>();
        if (_agent != null) return;
        _agent = bb.GetComponent<NavMeshAgent>();
        _transform = bb.transform;
        _anim = bb.GetComponentInChildren<Animator>();
        if (_sprite == null)
            _sprite = bb.GetComponentInChildren<SpriteRenderer>();
    }

    public override NodeState Tick(Blackboard bb)
    {
        //Debug.Log("ChargeAction.Tick 실행됨");
        EnsureInit(bb);

        var enemy = bb.GetComponent<EnemyController>();
        if (enemy == null || !enemy.IsAlive || !enemy.Agent.enabled)
        {
            if (_anim != null) _anim.SetBool("IsMoving", false);
            return NodeState.Failure;
        }

        //bb.IsPlayerVisible = true; 

        if (!bb.IsPlayerVisible)
            return NodeState.Failure;


        if (!_chargeStarted)
        {
            //Debug.Log("돌진 준비 중");
            float targetX = Random.Range(bb.Player.position.x - _chargePointRange, bb.Player.position.x + _chargePointRange);
            float targetZ = Random.Range(bb.Player.position.z - _chargePointRange, bb.Player.position.z + _chargePointRange);
            Vector3 chargeTarget = new Vector3(targetX, 0, targetZ);


            if (_chargeRange < Vector3.Distance(_transform.position, chargeTarget))
            {
                Debug.DrawLine(_transform.position, chargeTarget, Color.red, 1f);
                if (_anim != null)
                {
                    if(!attack.isCharging)
                    {
                        _anim.SetTrigger("IsAttack");
                    }
                    
                }
                attack.StartCharge(chargeTarget, _chargeSpeed, _chargeDuration, _chargeCoolTime);
                _chargeStarted = true;
            }
        }
        // 애니메이션
        if (_anim != null) _anim.SetBool("IsMoving", true);

        if (attack.isCharging == true)
            return NodeState.Running;

        _chargeStarted = false;
        return NodeState.Success;
    }
}


//public class ChargeAction : BTNode//이전코드 저장용?
//{
//    private readonly float _chargeRange;
//    private readonly float _chargeSpeed;
//    private readonly float _chargeDuration;
//    private readonly float _chargePointRange;
//    private NavMeshAgent _agent;
//    private Transform _transform;
//    private Animator _anim;
//    private SpriteRenderer _sprite;
//    private ChargeAttack attack;
//    private bool _chargeStarted = false;

//    public ChargeAction(float chargeRange, float chargeSpeed, float chargePointRange, float chargeDuraion)
//    {
//        _chargeRange = chargeRange;
//        _chargeSpeed = chargeSpeed;
//        _chargePointRange = chargePointRange;
//        _chargeDuration = chargeDuraion;
//    }
//    private void EnsureInit(Blackboard bb)
//    {
//        if (attack == null)
//            attack = bb.GetComponent<ChargeAttack>();
//        if (_agent != null) return;
//        _agent = bb.GetComponent<NavMeshAgent>();
//        _transform = bb.transform;
//        _anim = bb.GetComponentInChildren<Animator>();

//        if (_sprite == null)
//            _sprite = bb.GetComponentInChildren<SpriteRenderer>();
//    }

//    public void UseCharge(Blackboard bb)
//    {
//        if (attack.isCharging == true)
//            return;

//        float targetX = Random.Range(bb.Player.position.x - _chargePointRange, bb.Player.position.x + _chargePointRange);
//        float targetZ = Random.Range(bb.Player.position.z - _chargePointRange, bb.Player.position.z + _chargePointRange);

//        var _chargePoint = new Vector3(targetX, 0, targetZ);


//        if (_chargeRange < Vector3.Distance(_transform.position, _chargePoint))
//            attack.StartDash(_chargePoint, _chargeSpeed, _chargeDuration);
//    }

//    public override NodeState Tick(Blackboard bb)
//    {
//        Debug.Log("ChargeAction.Tick 실행됨");
//        EnsureInit(bb);

//        bb.IsPlayerVisible = true;

//        if (!bb.IsPlayerVisible)
//            return NodeState.Failure;


//        if (!_chargeStarted)
//        {
//            Debug.Log("돌진 준비 중");
//            //float targetX = Random.Range(bb.Player.position.x - _chargePointRange, bb.Player.position.x + _chargePointRange);
//            //float targetZ = Random.Range(bb.Player.position.z - _chargePointRange, bb.Player.position.z + _chargePointRange);
//            //Vector3 chargeTarget = new Vector3(targetX, 0, targetZ);
//            Vector3 dirToPlayer = (bb.Player.position - _transform.position).normalized;
//            Vector3 chargeTarget = _transform.position + dirToPlayer * _chargeRange;

//            Debug.DrawLine(_transform.position, chargeTarget, Color.red, 1f);
//            attack.StartDash(chargeTarget, _chargeSpeed, _chargeDuration);
//            _chargeStarted = true;

//            //if (_chargeRange < Vector3.Distance(_transform.position, chargeTarget))
//            //{
//            //    Debug.DrawLine(_transform.position, chargeTarget, Color.red, 1f);
//            //    attack.StartDash(chargeTarget, _chargeSpeed, _chargeDuration);
//            //    _chargeStarted = true;
//            //}
//        }
//        // 애니메이션
//        if (_anim != null) _anim.SetBool("IsMoving", true);

//        //_agent.speed = bb.GetComponent<EnemyController>().EnemyData.baseMoveSpeed * _speedMult;


//        if (attack.isCharging == true)
//            return NodeState.Running;

//        _chargeStarted = false;
//        return NodeState.Success;
//    }
//}
//public void UseCharge(Blackboard bb)
//{
//    if (attack.isCharging == true)
//        return;

//    float targetX = Random.Range(bb.Player.position.x - _chargePointRange, bb.Player.position.x + _chargePointRange);
//    float targetZ = Random.Range(bb.Player.position.z - _chargePointRange, bb.Player.position.z + _chargePointRange);

//    var _chargePoint = new Vector3(targetX, 0, targetZ);


//    if (_chargeRange < Vector3.Distance(_transform.position, _chargePoint))
//        attack.StartDash(_chargePoint, _chargeSpeed, _chargeDuration);
//}
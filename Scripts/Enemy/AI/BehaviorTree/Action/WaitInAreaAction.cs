using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WaitInAreaAction : BTNode//플레이어가 구역(SphereCollider)에 들어오면 Running반환 후 성공. 구역에서 나가면 실패반환
{
    private readonly float _areaRadius;
    public readonly float _waitTime;
    private NavMeshAgent _agent;
    private SphereCollider _sphereCollider;


    public WaitInAreaAction(float radius, float time)
    {
        _areaRadius = radius; 
        _waitTime = time;
    }
    private void EnsureInit(Blackboard bb)
    {
        if (_agent != null) return;
        _agent = bb.GetComponent<NavMeshAgent>();
        if (_sphereCollider == null)
            _sphereCollider = bb.GetComponentInChildren<SphereCollider>();
        bb.inTime = _waitTime;                //시간설정 = SO에서 설정한거
        _sphereCollider.radius = _areaRadius; //구역의 크기 = SO에서 설정한거
    }

    public override NodeState Tick(Blackboard bb)//Blackboard에 만들어둔 bool값을 전환 및 성공실패 반환
    {
        //Debug.Log("4th_Tick");
        EnsureInit(bb);
        if (!_sphereCollider.bounds.Contains(bb.Player.position))
        {
            //Debug.Log("Failure");

            bb.isPlayerIn = false;
            return NodeState.Failure;
        }
        
        if(_sphereCollider.bounds.Contains(bb.Player.position))
        {
            //Debug.Log("Running");
            bb.isPlayerIn = true; 
            return NodeState.Running;
        } 

        return NodeState.Success;
    }
}


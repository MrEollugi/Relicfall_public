using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class ChargeAttack : MonoBehaviour // ChargeAction : BTNode도 같은 오브젝트에 있어야함
{
    private Vector3 chargePoint; // 돌진할 위치
    private float chargeSpeed;// 돌진 속도
    private float chargeDuration;//돌진 지속시간
    private float coolTime;// 돌진 쿨타임 = SO에서 받은 쿨타임 저장용
    public float curCoolTime; // 현재 쿨타임

    public bool isCharging = false;//지금 돌진중?
    public bool isCoolDown = false;//쿨다운 = 트루 일땐 돌진 사용 불가능

    private NavMeshAgent agent;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public void StartCharge(Vector3 point, float speed, float duration, float time)// Action에서 돌진 가능할때 부르는메서드. 실제 이동은 업데이트에서
    {
        agent.Warp(transform.position);
        agent.velocity = Vector3.zero;
        agent.ResetPath();

        chargePoint = (point - transform.position).normalized;//방향 고정
        chargeSpeed = speed;
        chargeDuration = duration;
        coolTime = time;
        isCharging = true;

        curCoolTime = coolTime;

        //agent.updateRotation = false;
        agent.updatePosition = false;
    }

    private void Update()
    {
        if (isCharging == true && isCoolDown == false)//StartDash에서 차지 = 트루로 바꾸고 쿨다운중이 아닐때 대쉬
        {
            if(!agent || !agent.enabled || !agent.isOnNavMesh)
                return;

            chargeDuration -= Time.deltaTime;//지속시간 만큼 돌진
            if (chargeDuration > 0f)//돌진중일때
            {
                Vector3 move = chargePoint * chargeSpeed * Time.deltaTime;
                agent.Move(move);

                transform.position = agent.nextPosition;
            }
            else if(chargeDuration <= 0f)
            {
                isCharging = false; 
                isCoolDown = true;//돌진이 끝나고 쿨다운 시작

                agent.Warp(transform.position);
                agent.velocity = Vector3.zero;
                agent.ResetPath();

                agent.updatePosition = true;
            }
        }

        if (isCoolDown == true)
        {
            curCoolTime -= Time.deltaTime;
            if (curCoolTime <= 0f)
            {
                Debug.Log("쿨타임 0 이하로 떨어짐");
                isCharging = false;
                isCoolDown = false;
                curCoolTime = coolTime;
            }
        }
    }
    private void LateUpdate()
    {
        

    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BehaviorTree/Action/ChargeAction")]
public class ChargeActionSO : BTNodeSO
{
    [Tooltip("돌진 가능한 거리")]
    public float chargeRange = 10f;

    [Tooltip("돌진 속도")]
    public float chargeSpeed = 3f;

    [Tooltip("돌진 지속시간?")]
    public float chargeDuration = 3f;

    [Tooltip("돌진 좌표 랜덤범위, 없으면 0")]
    public float chargePointRange;

    [Tooltip("돌진 쿨타임")]
    public float chargeCoolTime;


    public override BTNode Build()
    {
        return new ChargeAction(chargeRange, chargeSpeed, chargePointRange, chargeDuration, chargeCoolTime);
    }
}

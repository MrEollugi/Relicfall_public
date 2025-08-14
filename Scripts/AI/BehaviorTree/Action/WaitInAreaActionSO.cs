using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BehaviorTree/Action/WaitInAreaAndAction")]
public class WaitInAreaActionSO : BTNodeSO
{
    [Tooltip("구역 크기")]
    public float areaRadius = 5f;

    [Tooltip("행동 시작까지 기다릴 시간")]
    public float waitTime = 5f;


    public override BTNode Build()
    {
        return new WaitInAreaAction(areaRadius, waitTime);
    }
}
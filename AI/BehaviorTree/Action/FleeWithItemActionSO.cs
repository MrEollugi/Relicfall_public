using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BehaviorTree/Action/FleeWithItem")]
public class FleeWithItemActionSO : BTNodeSO
{
    [Tooltip("도주 속도 계수")]
    public float fleeSpeedMultiplier = 1.5f;
    [Tooltip("도주 목표 위치")]
    public Vector3 safePoint;

    public override BTNode Build()
    {
        return new FleeWithItemAction(fleeSpeedMultiplier, safePoint);
    }
}

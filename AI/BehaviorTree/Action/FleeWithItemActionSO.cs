using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BehaviorTree/Action/FleeWithItem")]
public class FleeWithItemActionSO : BTNodeSO
{
    [Tooltip("���� �ӵ� ���")]
    public float fleeSpeedMultiplier = 1.5f;
    [Tooltip("���� ��ǥ ��ġ")]
    public Vector3 safePoint;

    public override BTNode Build()
    {
        return new FleeWithItemAction(fleeSpeedMultiplier, safePoint);
    }
}

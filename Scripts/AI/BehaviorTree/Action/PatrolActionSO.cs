using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BehaviorTree/Action/Patrol")]
public class PatrolActionSO : BTNodeSO
{
    [Tooltip("��Ʈ�� ������ �ݰ�")]
    public float patrolRadius = 5f;

    public override BTNode Build()
        => new PatrolAction(patrolRadius);
}

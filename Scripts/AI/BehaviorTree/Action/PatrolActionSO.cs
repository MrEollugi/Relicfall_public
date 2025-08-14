using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BehaviorTree/Action/Patrol")]
public class PatrolActionSO : BTNodeSO
{
    [Tooltip("패트롤 가능한 반경")]
    public float patrolRadius = 5f;

    public override BTNode Build()
        => new PatrolAction(patrolRadius);
}

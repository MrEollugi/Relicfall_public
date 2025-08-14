using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BehaviorTree/Action/SnatchPlayer")]
public class SnatchPlayerActionSO : BTNodeSO
{
    [Tooltip("���� �����Ÿ�")]
    public float catchRange = 1.5f;

    public override BTNode Build()
    {
        return new SnatchPlayerAction(catchRange);
    }
}

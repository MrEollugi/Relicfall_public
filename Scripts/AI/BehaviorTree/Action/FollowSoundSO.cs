using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BehaviorTree/Action/FollewSound")]
public class FollowSoundSO : BTNodeSO
{
    [Tooltip("?")]
    


    public override BTNode Build()
    {
        return new FollowSound();
    }


}

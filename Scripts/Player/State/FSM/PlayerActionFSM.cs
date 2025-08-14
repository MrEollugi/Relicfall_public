using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActionFSM
{
    private PlayerController player;
    private IActionState currentState;

    public PlayerActionFSM(PlayerController player)
    {
        this.player = player;
        currentState = new ActionIdleState(player, this);
    }

    public void Update(InputData input)
    {
        currentState?.Update(input);
    }

    public void ChangeState(IActionState newState, InputData input)
    {
        Debug.Log($"FSM ChangeState: {newState.GetType().Name}");
        currentState?.Exit();
        currentState = newState;
        currentState?.Enter(input);
    }

    public bool IsBusy => !(currentState is ActionIdleState);





    //public void TriggerSkill(SkillSO skill, InputData input)
    //{
    //    if(skill == null) return;

    //    ChangeState(new SkillExecutionState(player, this, skill), input);
    //}

    //public void TriggerSkillById(string skillId, InputData input)
    //{
    //    var skill = player.GetSkillById(skillId);
    //    if (skill != null)
    //    {
    //        TriggerSkill(skill, input);
    //    }
    //}

    //public void ForceHitCurrent()
    //{
    //    if (currentState is SkillExecutionState exec)
    //        exec.ForceHit();
    //}
}

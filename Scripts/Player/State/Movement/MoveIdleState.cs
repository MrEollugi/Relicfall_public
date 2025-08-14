using System.Linq;
using UnityEngine;

// Move FSM Idle
public class MoveIdleState : IMoveState
{
    private PlayerController player;
    private PlayerMoveFSM fsm;

    public MoveIdleState(PlayerController player, PlayerMoveFSM fsm)
    {
        this.player = player;
        this.fsm = fsm;
    }

    public void Enter()
    {
        player.BodyAnimatorManager.SetBool("isRunning", false);
        player.BodyAnimatorManager.SetBool("isSneaking", false);
        player.BodyAnimatorManager.SetBool("isMoving", false);
    }

    public void Update(InputData input)
    {
        if (player.IsMoveBlocked)
        {
            player.MoveDirection = Vector3.zero;
            player.MoveSpeed = 0f;
            player.UpdateAnimDirection(Vector3.zero);
            return;
        }

        if (input.isSneak)
        {
            fsm.ChangeState(new MoveSneakState(player, fsm));
            return;
        }

        if (input.MoveDirection.magnitude > 0.01f)
        {
            if (input.isRun && player.Stat.CurrentStamina > 0f && !player.IsStaminaExhausted)
            {
                fsm.ChangeState(new MoveRunState(player, fsm));
            }
            else
            {
                fsm.ChangeState(new MoveDefaultState(player, fsm));
            }
        }
    }

    public void Exit() { }
}

using System.Linq;
using UnityEngine;

// Normal Move (Walk)
public class MoveDefaultState : IMoveState
{
    private PlayerController player;
    private PlayerMoveFSM fsm;
    private float soundTimer = 0f;
    private float soundInterval = 0.425f;

    public MoveDefaultState(PlayerController player, PlayerMoveFSM fsm)
    {
        this.player = player;
        this.fsm = fsm;
    }

    public void Enter()
    {
        player.BodyAnimatorManager.SetBool("isMoving", true);
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

        soundTimer += Time.deltaTime;

        // Run / Sneak 진입 조건
        if (input.isRun && player.Stat.CurrentStamina > 0f && !player.IsStaminaExhausted)
        {
            fsm.ChangeState(new MoveRunState(player, fsm));
            return;
        }

        if (input.isSneak)
        {
            fsm.ChangeState(new MoveSneakState(player, fsm));
            return;
        }

        // Actual move
        Vector3 dir = new Vector3(input.MoveDirection.x, 0, input.MoveDirection.y).normalized;
        float baseSpeed = player.GetCurrentMoveSpeed(false, false);
        float speed = baseSpeed * player.MovementSpeedModifier;

        player.MoveDirection = dir;
        player.MoveSpeed = speed;

        player.UpdateAnimDirection(dir);

        // stop move
        if (input.MoveDirection.magnitude < 0.1f)
        {
            player.MoveDirection = Vector3.zero;
            player.MoveSpeed = 0f;
            fsm.ChangeState(new MoveIdleState(player, fsm));
            return;
        }

        // walk sfx sound system
        if (soundTimer >= soundInterval)
        {
            soundTimer = 0f;
            player.SoundEmitter.EmitMoveSound();
            SoundManager.Instance.PlaySFXAtPosition(
                player.GetRandomWalkClip(),
                player.transform.position,
                1f,        // volume
                10f,       // maxDistance
                0.08f      // pitch
            );
        }
    }

    public void Exit()
    {
        player.BodyAnimatorManager.SetBool("isMoving", false);
    }
}
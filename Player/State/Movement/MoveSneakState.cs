using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

// Move Sneak State
public class MoveSneakState : IMoveState
{
    private PlayerController player;
    private PlayerMoveFSM fsm;

    private float soundTimer = 0f;
    private float soundInterval = 0.75f;

    public MoveSneakState(PlayerController player, PlayerMoveFSM fsm)
    {
        this.player = player;
        this.fsm = fsm;
    }

    public void Enter()
    {
        player.BodyAnimatorManager.SetBool("isSneaking", true);
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

        if (!input.isSneak)
        {
            player.MoveDirection = Vector3.zero;
            player.MoveSpeed = 0f;
            if (input.MoveDirection.magnitude < 0.1f)
                fsm.ChangeState(new MoveIdleState(player, fsm));
            else
                fsm.ChangeState(new MoveDefaultState(player, fsm));
            return;
        }

        bool isMoving = input.MoveDirection.magnitude > 0.1f;
        player.BodyAnimatorManager.SetBool("isMoving", isMoving);

        if (isMoving)
        {
            Vector3 dir = new Vector3(input.MoveDirection.x, 0, input.MoveDirection.y).normalized;
            float baseSpeed = player.GetCurrentMoveSpeed(false, true);
            float speed = baseSpeed * player.MovementSpeedModifier;
            player.MoveDirection = dir;
            player.MoveSpeed = speed;

            soundTimer += Time.deltaTime;
            if (soundTimer >= soundInterval)
            {
                soundTimer = 0f;
                AudioClip sneakClip = player.GetRandomWalkClip();
                float pitch = Random.Range(0.04f, 0.07f);
                SoundManager.Instance.PlaySFX(sneakClip, pitch);
                SoundManager.Instance.PlaySFXAtPosition(
                    player.GetRandomWalkClip(),
                    player.transform.position,
                    1f,        // volume
                    10f,       // maxDistance
                    pitch      // pitch
                );
            }
        }
        else
        {
            player.MoveDirection = Vector3.zero;
            player.MoveSpeed = 0f;
            soundTimer = 0f;
        }
    }

    public void Exit()
    {
        player.BodyAnimatorManager.SetBool("isSneaking", false);
        player.BodyAnimatorManager.SetBool("isMoving", false);
    }
}

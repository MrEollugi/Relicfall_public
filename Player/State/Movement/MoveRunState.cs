using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Move Run State
public class MoveRunState : IMoveState
{
    private PlayerController player;
    private PlayerMoveFSM fsm;
    private float soundTimer;

    public MoveRunState(PlayerController player, PlayerMoveFSM fsm)
    {
        this.player = player;
        this.fsm = fsm;
    }

    public void Enter()
    {
        player.BodyAnimatorManager.SetBool("isRunning", true);
        soundTimer = 0f;
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

        // 이동 처리
        Vector3 dir = new Vector3(input.MoveDirection.x, 0, input.MoveDirection.y).normalized;
        float baseSpeed = player.GetCurrentMoveSpeed(true, false);
        float speed = baseSpeed * player.MovementSpeedModifier;

        player.MoveDirection = dir;
        player.MoveSpeed = speed;

        player.UpdateAnimDirection(dir);

        soundTimer += Time.deltaTime;
        if (soundTimer > 0.25f)
        {
            soundTimer = 0f;
            player.SoundEmitter.EmitRunSound(); // 필요하면 따로 추가

            AudioClip runClip = player.GetRandomRunClip();
            float pitch = Random.Range(0.12f, 0.17f);
            SoundManager.Instance.PlaySFX(runClip, pitch);
            SoundManager.Instance.PlaySFXAtPosition(
                player.GetRandomWalkClip(),
                player.transform.position,
                1f,        // volume
                10f,       // maxDistance
                pitch      // pitch
            );
        }

        // 1초에 10만큼 스태미나 감소
        float staminaCostPerSecond = 10f;
        float deltaStamina = staminaCostPerSecond * Time.deltaTime; 
        player.Stat.UseStamina(deltaStamina);

        // 스태미나 0이면 달리기 강제 종료
        if (player.Stat.CurrentStamina <= 0f)
        {
            player.SetStaminaExhausted();
            player.MoveDirection = Vector3.zero;
            player.MoveSpeed = 0f;
            fsm.ChangeState(new MoveIdleState(player, fsm));
            return;
        }

        if (input.MoveDirection == Vector2.zero || !input.isRun /*|| !player.HasStamina*/)
        {
            player.MoveDirection = Vector3.zero;
            player.MoveSpeed = 0f;
            fsm.ChangeState(new MoveIdleState(player, fsm));
        }
    }

    public void Exit()
    {
        player.BodyAnimatorManager.SetBool("isRunning", false);
    }

}

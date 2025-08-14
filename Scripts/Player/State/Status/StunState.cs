using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunState : IStatusState
{
    private readonly PlayerController player;
    private readonly PlayerStatusFSM fsm;
    private float timer;
    private readonly float breakDuration = 1.5f; // 원하는 기절 시간

    public StunState(PlayerController player, PlayerStatusFSM fsm, float breakDuration = 1.5f)
    {
        this.player = player;
        this.fsm = fsm;
        this.breakDuration = breakDuration;
    }

    public void Enter()
    {
        timer = 0f;
        player.BlockInput();
        player.PlayerRigidbody.velocity = Vector3.zero;

        if (SoundManager.Instance != null)
            SoundManager.Instance.PlaySFXAtPositionByClipName("GuardBreak", player.transform.position, 1.0f, 16f);

        // CameraShake.Instance.ShakeMedium();

        // 필요하면 UI 표시 등
    }

    public void Update(InputData input)
    {
        timer += Time.deltaTime;
        if (timer >= breakDuration)
        {
            player.UnblockInput();
            fsm.ChangeState(new StatusIdleState(player, fsm));
        }
    }

    public void Exit()
    {
    }
}

using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardBreakState : IStatusState
{
    private readonly PlayerController player;
    private readonly PlayerStatusFSM fsm;
    private float timer;
    private readonly float duration;
    private GameObject guardBreakEffectInstance;

    public GuardBreakState(PlayerController player, PlayerStatusFSM fsm, float duration)
    {
        this.player = player;
        this.fsm = fsm;
        this.duration = duration;
        Debug.Log($"[GuardBreakState] 경직시간: {duration}초");
    }

    public void Enter()
    {
        Debug.Log("[GuardBreakState] Enter");

        timer = 0f;
        player.BlockInput();
        player.BlockMove();
        player.PlayerRigidbody.velocity = Vector3.zero;

        // 머리 위에 스프라이트 이펙트
        if (player.GuardBreakEffect != null)
        {
            Vector3 headPos = player.transform.position + Vector3.up * 2.0f; // 원하는 위치로 조정
            guardBreakEffectInstance = GameObject.Instantiate(player.GuardBreakEffect, headPos, Quaternion.identity, player.transform);
        }

        // 사운드
        SoundManager.Instance?.PlaySFXAtPositionByClipName("GuardBreak", player.transform.position, 1.0f, 16f);

        var impulse = player.GetComponent<CinemachineImpulseSource>();
        if (impulse != null)
            impulse.GenerateImpulse();
    }

    public void Update(InputData input)
    {
        timer += Time.deltaTime;
        Debug.Log($"[GuardBreakState] timer: {timer}/{duration}");
        if (timer >= duration)
        {
            fsm.ChangeState(new StatusIdleState(player, fsm));
        }
    }

    public void Exit()
    {
        player.UnblockInput();
        player.UnblockMove();
        if (guardBreakEffectInstance != null)
            GameObject.Destroy(guardBreakEffectInstance);
    }
}

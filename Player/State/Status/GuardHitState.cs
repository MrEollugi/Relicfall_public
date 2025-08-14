using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardHitState : IStatusState
{
    private readonly PlayerController player;
    private readonly PlayerStatusFSM fsm;
    private float timer;
    private readonly float hitDuration = 0.05f;

    public GuardHitState(PlayerController player, PlayerStatusFSM fsm)
    {
        this.player = player;
        this.fsm = fsm; 
    }

    public void Enter()
    {
        timer = 0f;

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySFXAtPositionByClipName("GuardSound", player.transform.position, 0.8f, 16f);
        }

    }

    public void Update(InputData input)
    {
        timer += Time.deltaTime;
        if (timer >= hitDuration)
            fsm.ChangeState(new StatusIdleState(player, fsm));
    }

    public void Exit() { }
}

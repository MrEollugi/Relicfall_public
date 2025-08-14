using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusIdleState : IStatusState
{
    private readonly PlayerController player;
    private readonly PlayerStatusFSM fsm;

    public StatusIdleState(PlayerController player, PlayerStatusFSM fsm)
    {
        this.player = player;
        this.fsm = fsm;
    }

    public void Enter()
    {
    }

    public void Update(InputData input)
    {
    }

    public void Exit()
    {
        // 상태 이상 종료 시 처리도 가능
    }
}

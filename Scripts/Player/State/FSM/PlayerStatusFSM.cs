using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatusFSM
{
    private PlayerController player;
    private IStatusState currentState;
    public string CurrentStateName => currentState.GetType().Name;

    public PlayerStatusFSM(PlayerController player)
    {
        this.player = player;
        currentState = new StatusIdleState(player, this);
    }

    public void ChangeState(IStatusState newState)
    {
        Debug.Log($"[StatusFSM] State changed: {currentState?.GetType().Name} → {newState.GetType().Name}");
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    public void Update(InputData input)
    {
        currentState?.Update(input);
    }

    public bool IsBusy => !(currentState is StatusIdleState);
}

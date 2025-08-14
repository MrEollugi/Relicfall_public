using UnityEngine;
public class PlayerMoveFSM
{
    private PlayerController player;
    private IMoveState currentState;
    public IMoveState CurrentState => currentState;

    public PlayerMoveFSM(PlayerController player)
    {
        this.player = player;
        currentState = new MoveIdleState(player, this);
    }

    public void Update(InputData input)
    {
        currentState?.Update(input);
    }

    #region Change State
    public void ChangeState(IMoveState newState)
    {
        if (currentState != null)
            currentState.Exit();

        currentState = newState;

        if (currentState != null)
            currentState.Enter();
    }
    #endregion
}

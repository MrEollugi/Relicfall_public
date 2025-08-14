using UnityEngine;

public class HitState : IStatusState
{
    private readonly PlayerController player;
    private readonly PlayerStatusFSM fsm;
    private float timer;
    private readonly float hitDuration = 0.3f;

    public HitState(PlayerController player, PlayerStatusFSM fsm)
    {
        this.player = player;
        this.fsm = fsm;
    }

    public void Enter()
    {
        timer = 0f;
        //player.BodyAnimatorManager.SetTrigger("Hit");
        player.ShowDamageOverlay();

        if (SoundManager.Instance != null && player.HitClip != null)
            SoundManager.Instance.PlaySFXAtPosition(player.HitClip, player.transform.position, 0.08f, 16f);
        else
            Debug.LogWarning("[피격사운드] 재생 실패 - SoundManager 혹은 hitClip null");

        player.FlashRed(0.12f);
    }

    public void Update(InputData input)
    {
        timer += Time.deltaTime;
        if (timer >= hitDuration)
        {
            fsm.ChangeState(new StatusIdleState(player, fsm));
        }
    }

    public void Exit()
    {
        // 필요시 후처리
    }
}
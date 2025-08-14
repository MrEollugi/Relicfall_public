//using UnityEngine;

//public class MovementSkillState : IMoveState
//{
//    private SkillSO skill;
//    private Vector3 targetPosition;
//    private float duration;
//    private float timer;

//    public MovementSkillState(PlayerController player, PlayerFSM fsm, SkillSO skill, Vector3 targetPos) : base(player, fsm) { }

//    public void Enter()
//    {
//        Debug.Log($"Enter MovementSkill: {skill.DisplayName}");
//        // TODO: 애니메이션, 이펙트 시작
//    }

//    public override void Update(InputData input)
//    {
//        timer += Time.deltaTime;
//        float t = timer / duration;
//        player.transform.position = Vector3.Lerp(player.transform.position, targetPosition, t);

//        if (timer >= duration)
//        {
//            player.SkillController.UseSkill(skill);
//            fsm.ChangeState(new IdleState(player, fsm));
//        }
//    }

//    public void Exit()
//    {
//        Debug.Log($"Exit MovementSkill: {skill.DisplayName}");
//        // TODO: 이펙트 종료
//    }
//}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ActionIdleState : IActionState
{
    private PlayerController player;
    private PlayerActionFSM fsm;

    public ActionIdleState(PlayerController player, PlayerActionFSM fsm)
    {
        this.player = player;
        this.fsm = fsm;
    }

    public void Enter(InputData input) 
    {
        var anim = player.LeftWeaponController.Animator;
    }

    public void Update(InputData input)
    {
        if (input.isSpaceDown)
        {
            player.TryDash();
            return; // 한 번 입력만 반영
        }

        var rightClickSkillId = player.GetEquippedSkillId(ESkillSlotType.RightClick);
        var rightClickSkill = SkillRepository.Get(rightClickSkillId);

        bool isGuarding = input.isRightClickHeld
            && rightClickSkill != null
            && rightClickSkill.effects.Contains("guard");

        if (isGuarding) // 우클릭 or 가드 스킬 활성화 상태
        {
            player.RightWeaponController.SetGuardPosition(true);   // 방패 위치를 오른쪽으로
            player.GuardIndicator.SetActive(true);

        }
        else
        {
            player.RightWeaponController.SetGuardPosition(false);  // 원래 위치로 복귀
            player.GuardIndicator.SetActive(false);
        }

        // 1) Basic Attack
        if (!isGuarding) // Can Use when not guard
        {
            // 1) Basic Attack
            if (input.isAttackDown)
            {
                string skillId = player.GetEquippedSkillId(ESkillSlotType.BasicAttack);
                var skill = SkillRepository.Get(skillId);

                if (skill != null && player.Stat.CurrentStamina >= skill.staminaCost)
                {
                    player.UseSkill(skillId, player, GetSkillEffectValue(skill));
                }
            }
            // 2) E Skill
            else if (input.isESkillDown)
            {
                string skillId = player.GetEquippedSkillId(ESkillSlotType.E);
                var skill = SkillRepository.Get(skillId);
                if (skill != null && player.Stat.CurrentStamina >= skill.staminaCost)
                {
                    player.UseSkill(skillId, player, GetSkillEffectValue(skill));
                }
            }
        }

        if (input.isRightClickHeld)
        {
            if (rightClickSkill != null && rightClickSkill.effects.Contains("guard"))
            {
                player.UseSkill(rightClickSkillId, player, GetSkillEffectValue(rightClickSkill));
                player.IsGuarding = true;
                player.ApplyMovementModifier(rightClickSkill.guardMoveSpeedWhileGuard);
            }
            else if (rightClickSkill != null)
            {
                player.UseSkill(rightClickSkillId, player, GetSkillEffectValue(rightClickSkill));
            }
        }
        else
        {
            // Guard 해제
            if (rightClickSkill != null && rightClickSkill.effects.Contains("guard"))
            {
                player.IsGuarding = false;
                player.ResetMovementModifier();
            }
        }


        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        // 3) Q/E Skills
        //else if (input.isQSkill)
        //    player.TriggerSkill(player.GetEquippedSkill(ESkillSlotType.Q), input);


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        // 6) 즉시 상호작용(픽업/대화)
        //else if (input.isInteractDown)
        //{
        //    player.HandleImmediateInteraction();
        //}

        if (input.isInteract)
        {
            // Mouse cursor raycast
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit, 4f))
            {
                //// Timed Interact
                //if (hit.collider.TryGetComponent<ITimedInteractable>(out var timed))
                //{
                //    fsm.ChangeState(new SkillExecutionState(player, fsm, timed.InteractionSkillSO), input);
                //}
                // Immediate Interact
                if (hit.collider.TryGetComponent<IImmediateInteractable>(out var imm))
                {
                    imm.Interact(player);
                }
            }
        }
    }
    public void Exit() { }

    float GetSkillEffectValue(SkillData skill)
    {
        if (skill.effects.Contains("heal"))
            return skill.healParam;
        if (skill.effects.Contains("damage"))
            return skill.damageParam;
        // 필요하다면 customParams 등
        return 0;
    }
}

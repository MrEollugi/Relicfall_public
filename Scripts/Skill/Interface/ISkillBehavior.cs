using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISkillBehavior
{
    void OnEnter(PlayerController player, InputData input);
    void OnUpdate(PlayerController player, InputData input, float deltaTime);
    void OnExit(PlayerController player);

    bool IsComplete {  get; }

    void ForceExecuteHit(PlayerController player);

    void OnHit(PlayerController player, ref float damage, Vector3 attackDirection);
}

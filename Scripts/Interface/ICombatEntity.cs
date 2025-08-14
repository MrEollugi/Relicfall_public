using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICombatEntity
{
    IStat Stat { get; }
    Transform Transform { get; }
    bool IsAlive { get; }

    void TakeDamage(float damage, Vector3 attackDirection = default);
    float GetAttackPower();
    void ApplyStagger(float value);

    void Heal(float amount);

    #region Status Effect

    void ApplyStun(float duration);
    void ApplyBleed(float amount);

    #endregion

}

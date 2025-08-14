using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance { get; private set; }
    private List<ICombatEntity> enemies = new List<ICombatEntity>();

    private void Awake() => Instance = this;

    public void RegisterEnemy(ICombatEntity enemy) => enemies.Add(enemy);
    public void UnregisterEnemy(ICombatEntity enemy) => enemies.Remove(enemy);

    // 실제 범위/각도 판정 함수
    public List<ICombatEntity> FindEnemiesInArea(Vector3 origin, float range, float angle, Vector3 forward)
    {
        var result = new List<ICombatEntity>();
        foreach (var e in enemies)
        {
            if (!e.IsAlive) continue;
            Vector3 dir = e.Transform.position - origin;
            if (dir.magnitude > range) continue;
            if (angle < 360f)
            {
                float a = Vector3.Angle(forward, dir);
                if (a > angle * 0.5f) continue;
            }
            result.Add(e);
        }
        return result;
    }
}

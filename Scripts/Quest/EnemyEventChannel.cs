using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EnemyEventChannel
{
    public static event Action<string> OnEnemyKilled;

    public static event Action<string> OnBossKilled;

    public static void RaiseEnemyKilled(string enemyName) => OnEnemyKilled?.Invoke(enemyName);

    public static void RaiseBossKilled(string bossName) => OnBossKilled?.Invoke(bossName);
}

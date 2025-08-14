using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ProjectilePrefabRegistry
{
    public static readonly Dictionary<string, GameObject> PrefabMap = new();

    // 게임 시작 시 프리팹 등록 (Addressable, Resources, Inspector 등 방식에 맞게)
    public static void Initialize()
    {
        PrefabMap["BasicBullet"] = Resources.Load<GameObject>("Projectile/BasicBullet");
        PrefabMap["Test_Projectile"] = Resources.Load<GameObject>("Projectile/Test_Projectile");
        PrefabMap["Projectile_Shotgun"] = Resources.Load<GameObject>("Projectile/Projectile_Shotgun");
        PrefabMap["DMRBullet"] = Resources.Load<GameObject>("Projectile/DMRBullet");
        // ... etc
    }
}

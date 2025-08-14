using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DeadPrefabRegistry
{
    private static Dictionary<string, GameObject> deadPrefabMap = new();

    public static void Init()
    {
        deadPrefabMap["default"] = Resources.Load<GameObject>("Dead/Dead_Default");
        deadPrefabMap["job_knight"] = Resources.Load<GameObject>("Dead/Dead_Knight");
        deadPrefabMap["job_wizard"] = Resources.Load<GameObject>("Dead/Dead_Wizard");
        // ... etc
    }

    public static GameObject Get(string jobId)
    {
        if (deadPrefabMap.TryGetValue(jobId, out var prefab))
            return prefab;
        // 기본값(예: Head_Default) 반환
        return deadPrefabMap["default"];
    }
}

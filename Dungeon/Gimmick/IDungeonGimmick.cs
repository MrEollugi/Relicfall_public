using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 발동형 기믹 interface
public interface IDungeonGimmick
{
    void Initialize(DungeonThemeData themeData);

    void OnTriggerEnter(PlayerController player);
}

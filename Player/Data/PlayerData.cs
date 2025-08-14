using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Jobs.LowLevel.Unsafe;
using UnityEngine;

[Serializable]
public class PlayerData
{
    [Header("Job Info")]
    public string JobId = "job_knight";

    public string Nickname ="";

    [Header("Level Info")]
    public int Level = 1;

    [Header("Money")]
    public int whereIsGold = 0;

    #region Runtime Stat Status
    public float CurrentHP;
    public float CurrentMP;
    public float CurrentStamina;

    #endregion

    public long PlayTimeSeconds = 0;

    [Header("Inventory")]
    public InventoryData Inventory = new InventoryData();
}

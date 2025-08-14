using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InventoryData
{
    [Header("Grid Size")]
    public int Width = 5;
    public int Height = 4;

    [Header("Stored Items")]
    public List<ItemInstanceSaveData> Items = new List<ItemInstanceSaveData>();
}

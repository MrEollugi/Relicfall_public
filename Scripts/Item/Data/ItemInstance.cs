using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemInstanceSaveData
{
    public string itemId;
    public int stackCount;
    public int anchorX;
    public int anchorY;
    public int actualValue;
}
public class ItemInstance
{
    [SerializeField] private string instanceId = Guid.NewGuid().ToString();
    public Guid InstanceId => Guid.Parse(instanceId);

    public ItemData data;

    public string itemId;

    public int stackCount;
    public int AnchorX { get; set; }
    public int AnchorY { get; set; }
    public int ActualValue { get; private set; }

    // 아래처럼 property 변경
    public int SizeX => data.sizeX;
    public int SizeY => data.sizeY;
    public bool IsStackable => data.isStackable;
    public int MaxStackCount => data.maxStackCount;
    public string Id => data.id;

    public string ownerPlayerId;

    #region 생성자
    // JSON 데이터
    public ItemInstance(ItemData data)
    {
        this.data = data;
        itemId = data.id;
        stackCount = 1;
        ActualValue = data.value + UnityEngine.Random.Range(-data.valueRange, data.valueRange + 1);
    }

    // 아이템 id
    public ItemInstance(string itemId)
    {
        this.data = ItemDatabase.Instance.GetItemById(itemId);
        this.itemId = itemId;
        stackCount = 1;
        ActualValue = data.value + UnityEngine.Random.Range(-data.valueRange, data.valueRange + 1);
    }
    #endregion

    #region 세이브 & 로드
    public ItemInstanceSaveData ToSaveData()
    {
        return new ItemInstanceSaveData
        {
            itemId = data.id,
            stackCount = stackCount,
            anchorX = AnchorX,
            anchorY = AnchorY,
            actualValue = ActualValue
        };
    }

    public static ItemInstance FromSaveData(ItemInstanceSaveData save, ItemData itemData)
    {
        var inst = new ItemInstance(itemData)
        {
            stackCount = save.stackCount,
            AnchorX = save.anchorX,
            AnchorY = save.anchorY,
            ActualValue = save.actualValue
        };
        return inst;
    }
    #endregion
}

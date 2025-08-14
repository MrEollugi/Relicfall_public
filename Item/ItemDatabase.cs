using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class ItemDatabase
{
    private static ItemDatabase _instance;
    public static ItemDatabase Instance => _instance ??= new ItemDatabase();

    private const string ItemFileName = "item.json";

    private Dictionary<string, ItemData> idMap = new();
    private List<ItemData> items;

    public void Init()
    {
        string path = Path.Combine(Application.persistentDataPath, ItemFileName);

        if (!File.Exists(path))
            return;

        string json = File.ReadAllText(path);
        items = JsonUtility.FromJson<ItemDataList>(json).items;
        idMap = items.ToDictionary(item => item.id, item => item);
    }

    public ItemData GetItemById(string id)
    {
        if (idMap == null || idMap.Count == 0)
            return null;

        return idMap.TryGetValue(id, out var data) ? data : null;
    }
}

[System.Serializable]
public class ItemDataList
{
    public List<ItemData> items;
}
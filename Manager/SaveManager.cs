using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Cinemachine.CinemachineTriggerAction.ActionSettings;


public enum StorageType
{
    Hub,
    Wagon
}

// for Single play save data
[Serializable]
public class WorldData
{
    public int TotalGold = 200;
    public int TargetGold;
    public List<ItemList> storagesList = new();
    public QuestSaveData questSaveData;
    // TODO: WorldProgress, CompleteQuests etc

    [NonSerialized]
    public Dictionary<string, List<ItemInstanceSaveData>> Storages = new();

    public void SyncStoragesToList()
    {
        storagesList.Clear();
        foreach (var kv in Storages)
        {
            storagesList.Add(new ItemList
            {
                key = kv.Key,
                value = kv.Value
            });
        }
    }

    public void SyncListToStorages()
    {
        Storages.Clear();
        foreach (var pair in storagesList)
        {
            Storages[pair.key] = pair.value;
        }
    }
}

[Serializable]
public class ItemList
{
    public string key;
    public List<ItemInstanceSaveData> value;
}

// PlayerData를 JSON으로 저장하고(SavePlayer()), 게임 시작 또는 거점 씬 진입 시 불러오는 역할 
public class SaveManager
{

    private const string SinglePlayerFileFormat = "single_player_{0}.json";
    private const string SingleWorldFileFormat = "single_world_{0}.json";
    private const string MultiPlayerFileFormat = "multi_player_{0}.json";
    private const string MultiWorldFileFormat = "multi_world_{0}.json";

    // 싱글톤 대신, GameManager에서 직접 생성·주입
    public SaveManager() { }

    public void SavePlayer(PlayerData data, string id, GameMode mode)
    {
        string fileName = mode == GameMode.Single
            ? $"single_player_{id}.json"
            : $"multi_player_{id}.json";
        string path = Path.Combine(Application.persistentDataPath, fileName);
        string json = JsonUtility.ToJson(data, prettyPrint: true);
        File.WriteAllText(path, json);
    }

    public PlayerData LoadPlayer(string id, GameMode mode)
    {
        string fileName = mode == GameMode.Single
            ? $"single_player_{id}.json"
            : $"multi_player_{id}.json";
        string path = Path.Combine(Application.persistentDataPath, fileName);
        if (!File.Exists(path))
            return null;
        string json = File.ReadAllText(path);
        return JsonUtility.FromJson<PlayerData>(json);
    }

    public void SaveWorld(WorldData data, string id, GameMode mode)
    {
        data.SyncStoragesToList();

        string fileName = mode == GameMode.Single
            ? $"single_world_{id}.json"
            : $"multi_world_{id}.json";
        string path = Path.Combine(Application.persistentDataPath, fileName);
        string json = JsonUtility.ToJson(data, prettyPrint: true);
        File.WriteAllText(path, json);
    }


    public WorldData LoadWorld(string id, GameMode mode)
    {
        string fileName = mode == GameMode.Single
            ? $"single_world_{id}.json"
            : $"multi_world_{id}.json";
        string path = Path.Combine(Application.persistentDataPath, fileName);
        if (!File.Exists(path))
            return null;
        string json = File.ReadAllText(path);
        var data = JsonUtility.FromJson<WorldData>(json);

        data.SyncListToStorages();

        return data;
    }
}

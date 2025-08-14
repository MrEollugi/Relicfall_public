using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipPrefabRegistry : MonoBehaviour
{
    public static TooltipPrefabRegistry Instance {  get; private set; }

    [System.Serializable]
    public class TooltipEntry
    {
        public TooltipType type;
        public GameObject prefab;
    }

    [SerializeField] private List<TooltipEntry> tooltipPrefabs;

    private Dictionary<TooltipType, GameObject> prefabMap;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        prefabMap = new Dictionary<TooltipType, GameObject>();
        foreach (var entry in tooltipPrefabs)
        {
            if (!prefabMap.ContainsKey(entry.type) && entry.prefab != null)
                prefabMap.Add(entry.type, entry.prefab);
        }
    }

    public GameObject GetTooltipPrefab(TooltipType type) 
    {
        if (prefabMap.TryGetValue(type, out var prefab))
            return prefab;

        return null;
    }
}

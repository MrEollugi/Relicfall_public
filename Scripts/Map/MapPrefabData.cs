using UnityEngine;

[CreateAssetMenu(fileName = "MapPrefabData", menuName = "Scriptable Objects/Data/Map Prefab Data")]
public class MapPrefabData : ScriptableObject
{
    [field: SerializeField] public MapPrefab[] prefabs { get; private set; }
}

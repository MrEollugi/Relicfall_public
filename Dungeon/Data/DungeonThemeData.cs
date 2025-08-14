using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DungeonTheme
{
    Underground,
    // Cave,
    // SnowField,
    Temple,
}

[CreateAssetMenu(fileName = "DungeonThemeData", menuName = "Scriptable Objects/Data/Dungeon Theme Data")]
public class DungeonThemeData : ScriptableObject
{
    [Header("DungeonTheme")]
    public DungeonTheme dungeonTheme;

    [Header("Map Size")]
    public int minCount;
    public int maxCount;

    [Header("Sight")]
    public float sightMultiplier; // 시야 정도
    public float depthBrightnessDecay;  // 깊이에 따른 밝기 감소 정도
    public float baseAmbientIntensity = 1f; // 기본 광원 세기

    [Header("Prefabs")]
    public GameObject[] trapPrefab;
    public GameObject KeyDoorPrefab;
    public GameObject lightSourcePrefab;
    public MapPrefabData mapPrefabData;
}
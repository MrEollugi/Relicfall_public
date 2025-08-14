using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonMapData : MonoBehaviour
{
    public string mapName;
    public DungeonThemeData themeData;      // 연결된 테마
    public int minRooms = 20;
    public int maxRooms = 40;
    public int seed = 0;
    public bool useFixedSeed = false;
    // 필요하다면 특수방, 보스방, 보상방 등의 옵션도 추가!
    public bool hasBossRoom;
    public int trapRoomCount;
}

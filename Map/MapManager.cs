using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
// using Unity.VisualScripting;
using UnityEngine;

public enum MapPrefabType
{
    SmallRoom, MediumRoom, LargeRoom,

    HorizontalPassage, VerticalPassage,
    LDRTPassage, LUDTPassage, RUDTPassage, RULTPassage,
    LDCornerPassage, URCornerPassage, DRCornerPassage, ULCornerPassage,
    CrossPassage,
}

public class MapManager : MonoBehaviour
{
    // 받아오는 방식 수정이 필요할 듯
    public DungeonThemeData[] dungeonThemeDatas;
    [SerializeField] private DungeonTheme curDungeonTheme;
    [SerializeField] private DungeonThemeData curDungeonData;
    
    MapPrefabData mapPrefabData;

    [SerializeField] MapNavMeshBaker navMeshBaker;
    [SerializeField] GameObject DoorPrefab;
    private Dictionary<MapPrefabType, GameObject> mapPrefabs = new();
    private Dictionary<MapNode, GameObject> roomInstances = new();

    // 노드 방문 
    public event Action<MapNode> OnNodeVisited;
    private Dictionary<Vector2Int, MapNode> nodeLookup = new();

    public int GetTotalNodeCount() => nodeLookup.Values.Count;
    public int GetVisitedNodeCount() => nodeLookup.Values.Count(n => n.isVisited);
    public void NotifyNodeVisited(MapNode node) => node.Visit();

    // private Dictionary<Vector2Int, MiniMapNode> minimapDic = new();
    public Sprite visitedSprite;
    public Sprite unvisitedSprite;

    // private static readonly List<Vector2Int> directions = new()
    // {
    //     new Vector2Int(0, 1),   // U
    //     new Vector2Int(0, -1),  // D
    //     new Vector2Int(-1, 0),  // L
    //     new Vector2Int(1, 0)    // R
    // };

    protected void Awake()
    {
        foreach (var d in dungeonThemeDatas)
        {
            if (d.dungeonTheme == curDungeonTheme)
            {
                mapPrefabData = d.mapPrefabData;
                curDungeonData = d;
                break;
            }
        }

        // var mapPrefabData = Resources.Load<MapPrefabData>("MapPrefabData");

        if (mapPrefabData == null)
        {
            Debug.LogError("잘못된 리소스");

            return;
        }

        foreach (var prefab in mapPrefabData.prefabs)
        {
            if (!mapPrefabs.ContainsKey(prefab.mapPrefabType))
            {
                mapPrefabs.Add(prefab.mapPrefabType, prefab.mapPrefab);
            }
        }

        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.RegisterMapManager(this);
        }

        // OnNodeVisited += node =>
        // {
        //     if (minimapDic.TryGetValue(node.position, out var minimap))
        //         minimap.SetVisited();
        // };
    }

    void Start()
    {
        StartCoroutine(GenerateBake());
    }

    void BuildMap(MapGraph graph)
    {
        // 각 MapNode의 정보를 기반으로 적절한 프리팹 선택
        // Room 내부 node들 추출
        var roomInterior = graph.Nodes.Where(n => n.type == NodeType.Room)
        .SelectMany(n => GetInteriorCells(n)).ToHashSet();

        foreach (var node in graph.Nodes)
        {
            RegisterNode(node);

            // 프리팹 배치: Instantiate()
            if (node.type is NodeType.Room)
            {
                GameObject newRoom = Instantiate(SetRoom(node), MapUtils.GridToWorld(node.position),
                Quaternion.identity, transform);

                // 트리거 컴포넌트
                var trigger = newRoom.AddComponent<NodeVisitTrigger>();
                trigger.Initialize(node, this);

                // // 미니맵용 SpriteRenderer
                // var minimap = newRoom.AddComponent<MiniMapNode>();
                // minimap.visitedSprite = visitedSprite;
                // minimap.unvisitedSprite = unvisitedSprite;
                // minimap.Initialize(node.isVisited);

                // minimapDic[node.position] = minimap;
                roomInstances[node] = newRoom;
            }
            else if (node.type is NodeType.Passage)
            {
                if (roomInterior.Contains(node.position))
                    continue;

                BuildPassage(node);
            }
        }

        SetDoor(graph);
    }

    // 방 내부인지 체크
    private IEnumerable<Vector2Int> GetInteriorCells(MapNode room)
    {
        int side = MapUtils.GetSideLength(room.size.Value);

        for (int x = 0; x < side; x++)
            for (int y = 0; y < side; y++)
                yield return new Vector2Int(room.position.x + x, room.position.y + y);
    }

    GameObject SetRoom(MapNode node)
    {
        if (node.size is RoomSizeType.Small)
        {
            if (!mapPrefabs.TryGetValue(MapPrefabType.SmallRoom, out var prefab))
            {
                Debug.LogError("No SmallRoom Prefabs");
                return null;
            }

            return prefab;
        }
        else if (node.size is RoomSizeType.Medium)
        {
            if (!mapPrefabs.TryGetValue(MapPrefabType.MediumRoom, out var prefab))
            {
                Debug.LogError("No MediumRoom Prefabs");
                return null;
            }

            return prefab;
        }
        else
        {
            if (!mapPrefabs.TryGetValue(MapPrefabType.LargeRoom, out var prefab))
            {
                Debug.LogError("No LargeRoom Prefabs");
                return null;
            }

            return prefab;
        }
    }

    void BuildPassage(MapNode n)
    {
        bool U = n.connectDir.Contains(Direction.Up);
        bool D = n.connectDir.Contains(Direction.Down);
        bool L = n.connectDir.Contains(Direction.Left);
        bool R = n.connectDir.Contains(Direction.Right);

        Vector3 pos = MapUtils.GridToWorld(n.position);

        GameObject newPassage = new GameObject();

        if (U && D && !L && !R)
            newPassage = Instantiate(mapPrefabs[MapPrefabType.VerticalPassage], pos, Quaternion.identity, transform);
        else if (L && R && !U && !D)
            newPassage = Instantiate(mapPrefabs[MapPrefabType.HorizontalPassage], pos, Quaternion.identity, transform);
        else if (!U && !R && D && L)
            newPassage = Instantiate(mapPrefabs[MapPrefabType.LDCornerPassage], pos, Quaternion.identity, transform);
        else if (!R && !D && U && L)
            newPassage = Instantiate(mapPrefabs[MapPrefabType.ULCornerPassage], pos, Quaternion.identity, transform);
        else if (!D && !L && U && R)
            newPassage = Instantiate(mapPrefabs[MapPrefabType.URCornerPassage], pos, Quaternion.identity, transform);
        else if (!L && !U && R && D)
            newPassage = Instantiate(mapPrefabs[MapPrefabType.DRCornerPassage], pos, Quaternion.identity, transform);
        else if (L && D && R && !U)
            newPassage = Instantiate(mapPrefabs[MapPrefabType.LDRTPassage], pos, Quaternion.identity, transform);
        else if (L && D && !R && U)
            newPassage = Instantiate(mapPrefabs[MapPrefabType.LUDTPassage], pos, Quaternion.identity, transform);
        else if (!L && D && R && U)
            newPassage = Instantiate(mapPrefabs[MapPrefabType.RUDTPassage], pos, Quaternion.identity, transform);
        else if (L && !D && R && U)
            newPassage = Instantiate(mapPrefabs[MapPrefabType.RULTPassage], pos, Quaternion.identity, transform);
        else
            newPassage = Instantiate(mapPrefabs[MapPrefabType.CrossPassage], pos, Quaternion.identity, transform);

        var trigger = newPassage.AddComponent<NodeVisitTrigger>();
        trigger.Initialize(n, this);
    }

    void SetDoor(MapGraph graph)
    {
        // 내부 시작 방 > 외부 맵으로 이동할 문 (입구)
        foreach (var r in roomInstances)
        {
            if (r.Key.position == Vector2Int.zero)
            {
                GameObject doorObj = Instantiate(DoorPrefab, MapUtils.GetDoorPosition(RoomSizeType.Medium), Quaternion.identity, roomInstances[r.Key].transform);
                Door mainDoor = doorObj.GetComponent<Door>();
                mainDoor.pairId = "MainDoor";
                break;
            }

        }

        // 내부 제일 먼 방 > 외부 맵으로 이동할 문 (출구)
        MapNode farthestRoom = FindFarthestRoom(new Vector2Int(0, 0), graph);
        Vector3 farthestDoorPos = MapUtils.GridToWorld(farthestRoom.position) + MapUtils.GetDoorPosition(farthestRoom.size ?? RoomSizeType.Medium);
        GameObject subDoorObj = Instantiate(DoorPrefab, farthestDoorPos, Quaternion.identity, roomInstances[farthestRoom].transform);
        Door subDoor = subDoorObj.GetComponent<Door>();
        subDoor.pairId = "SubDoor";
    }

    MapNode FindFarthestRoom(Vector2Int start, MapGraph graph)
    {
        // Dictionary<Vec2Int, MapNode> roomNodes
        var roomNodes = graph.Nodes.Where(n => n.type == NodeType.Room).ToDictionary(n => n.position, n => n);
        
        MapNode farthestNode = null!;
        int maxSqrDist = -1;

        foreach (var r in roomNodes)
        {
            Vector2Int pos = r.Key;
            Vector2Int delta = pos - start;

            int sqrDist = delta.x * delta.x + delta.y * delta.y;

            if (sqrDist > maxSqrDist)
            {
                maxSqrDist = sqrDist;
                farthestNode = r.Value;
            }
        }

        return farthestNode;
    }

    public void RegisterNode(MapNode node)
    {
        if (!nodeLookup.ContainsKey(node.position))
        {
            nodeLookup.Add(node.position, node);
            node.OnVisited += n => OnNodeVisited?.Invoke(n);
        }
    }


    IEnumerator GenerateBake()
    {
        MapGraphGenerator generator = new MapGraphGenerator();
        MapGraph graph = generator.MapGenerate(curDungeonData.minCount, curDungeonData.maxCount);

        BuildMap(graph);

        yield return new WaitForEndOfFrame();

        navMeshBaker.Bake();
    }
}

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapGraphGenerator
{
    private int totalRoomCount;
    private List<MapNode> nodes = new();
    private HashSet<Vector2Int> roomBlocked = new();

    private Dictionary<Vector2Int, Vector2Int> parentMap = new();


    public MapGraph MapGenerate(int min, int max)
    {
        nodes.Clear();
        roomBlocked.Clear();
        parentMap.Clear();

        // 시작 방 배치
        AddRoom(Vector2Int.zero, RoomSizeType.Medium, Vector2Int.zero);

        // Branch 개수 나누기
        totalRoomCount = Random.Range(min, max);
        int firstRoomCount = Random.Range(5, min);
        int secondRoomCount = Random.Range(5, min);
        int thirdRoomCount = Mathf.Max(0, totalRoomCount - firstRoomCount - secondRoomCount);

        // Branch 생성
        Generate(firstRoomCount, Vector2Int.left, Vector2Int.zero, RoomSizeType.Small);
        Generate(secondRoomCount, Vector2Int.right, Vector2Int.zero, RoomSizeType.Small);
        Generate(thirdRoomCount, Vector2Int.down, Vector2Int.zero, RoomSizeType.Small);

        // 방끼리 A* 연결
        ConnectRoom();

        return new MapGraph(nodes); 
    }

    void Generate(int roomCount, Vector2Int currentPos, Vector2Int parentPos, RoomSizeType parentSize)
    {
        if (roomCount <= 0) return;

        Vector2Int curDir = currentPos;
        Vector2Int curOrigin = parentPos;
        RoomSizeType curSize = parentSize;

        int safeCounter = 0;
        while (roomCount > 0 && safeCounter < 1000)
        {
            safeCounter++;

            // 다음 방 크기 선택
            RoomSizeType nextSize = GetRandomSize();

            // 처음엔 분기 방향으로 방 생성
            // 이후엔 역방향 제외 랜덤 방향
            if (roomCount > 1)
                curDir = MapUtils.GetRandomDirectionExclude(-curDir);

            // 이동할 칸 수 = 현재 방 사이즈 + 방과 방 사이의 1칸 버퍼 + 다음 방 사이즈
            int step = MapUtils.GetSideLength(curSize) + 1 + MapUtils.GetSideLength(nextSize); 
            Vector2Int nextOrigin = curOrigin + curDir * step;

            // 영역 확인 (충돌 체크)
            if (IsAreaFree(nextOrigin, nextSize))
            {
                // Room 추가 및 등록
                AddRoom(nextOrigin, nextSize, curOrigin);

                // 변수 갱신
                roomCount--;
                curOrigin = nextOrigin;
                curSize = nextSize;

                // 브랜치에 서브 브랜치를 생성
                int subBrances = Random.Range(0, 3);
                
                for (int i = 0; i < subBrances; i++)
                {
                    Vector2Int branchDir = MapUtils.GetRandomDirectionExclude(Vector2Int.zero);
                    int subBranchCount = Random.Range(0, roomCount + 1);
                    roomCount -= subBranchCount;
                    // 재귀 생성
                    Generate(subBranchCount, branchDir, curOrigin, curSize);
                }
            }
        }
    }

    // 방과 버퍼 영역이 비었는지 확인
    private bool IsAreaFree(Vector2Int origin, RoomSizeType size)
    {
        foreach (var cell in IterateArea(origin, size, buffer : 1))
            if (roomBlocked.Contains(cell)) return false;
        return true;
    }

    private void AddRoom(Vector2Int origin, RoomSizeType size, Vector2Int parentOrigin)
    {
        nodes.Add(new MapNode(NodeType.Room, origin, size));

        parentMap[origin] = parentOrigin;

        // 버퍼 포함 영역 돌며 방 영역으로 추가
        foreach (var cell in IterateArea(origin, size, buffer: 1))
        {
            roomBlocked.Add(cell);
        }    
    }

    // 영역 순회
    private IEnumerable<Vector2Int> IterateArea(Vector2Int origin, RoomSizeType size, int buffer)
    {
        int s = MapUtils.GetSideLength(size);
        for (int x = -buffer; x < s + buffer; x++)
            for (int y = -buffer; y < s + buffer; y++)
                yield return new Vector2Int(origin.x + x, origin.y + y);
    }

    // 방과 방 사이의 경로
    void ConnectRoom()
    {
        var roomList = nodes.Where(n => n.type == NodeType.Room).ToList();
        if (roomList.Count <= 1) return;

        // 경계 설정 (무한계산 방지)
        int margin = 6;
        int minX = roomList.Min(n => n.position.x) - margin;
        int maxX = roomList.Max(n => n.position.x) + margin;
        int minY = roomList.Min(n => n.position.y) - margin;
        int maxY = roomList.Max(n => n.position.y) + margin;
        bool OutOfBounds(Vector2Int p) => p.x < minX || p.x > maxX || p.y < minY || p.y > maxY;

    // 1번 경로 (복도가 길고 복잡함)
        // 기존 순차 연결 로직
        for (int i = 1; i < roomList.Count; i++)
        {
            var start = roomList[i - 1];
            var goal = roomList[i];

            var blocked = BuildBlocked(start, goal);

            // A*
            var path = Pathfinder.AStar(start.position, goal.position, blocked, OutOfBounds);
            if (path == null)
                continue;

            AddPassage(path);
        }

    // 2번 경로 (복도가 짧고 단순함) 
    //    // 시작 방에서 3 브랜치로 나누어 복도 생성 로직
    //     // 시작 방 origin
    //     Vector2Int startOrigin = Vector2Int.zero;

    //     // 시작 방의 직계 자식만 골라내기
    //     var firstNodes = parentMap.Where(kv => kv.Value == startOrigin && kv.Key != startOrigin).Select(kv => kv.Key);

    //     // 시작 방 A* 돌려서 복도 연결
    //     var startNode = nodes.First(n => n.position == startOrigin);
    //     foreach (var child in firstNodes)
    //     {
    //         var childNode = nodes.First(n => n.position == child);

    //         var blocked = BuildBlocked(startNode, childNode);

    //         var path = Pathfinder.AStar(startOrigin, child, blocked, OutOfBounds);
    //         if (path == null)
    //             continue;
    //         AddPassage(path);
    //     }
        
    //     // 나머지 복도 연결
    //     foreach (var kv in parentMap)
    //     {
    //         var child = kv.Key;
    //         var parent = kv.Value;

    //         if (child == startOrigin || parent == startOrigin)
    //             continue;   

    //         var pNode = nodes.First(n => n.position == parent);
    //         var cNode = nodes.First(n => n.position == child);

    //         var blocked2 = BuildBlocked(pNode, cNode);
    //         var path2 = Pathfinder.AStar(parent, child, blocked2, OutOfBounds);

    //         if (path2 != null)
    //             AddPassage(path2);
    //     }
    }

    // 경로에 따라 복도 노드 생성 
    private void AddPassage(List<Vector2Int> path)
    {
        for (int i = 0; i < path.Count; i++)
        {
            Vector2Int pos = path[i];
            bool isEnd = (i == 0 || i == path.Count - 1);

            if (isEnd)
            {
                //  연결 정보
                Vector2Int next = (i == 0) ? path[1] : path[path.Count - 2];
                Direction dir = MapUtils.Vector2IntToDirection(next - pos);
                Connect(pos, dir);
            }
            else
            {
                // 복도 노드 추가
                var passage = nodes.FirstOrDefault(n => n.position == pos);
                if (passage == null)
                {
                    passage = new MapNode(NodeType.Passage, pos);
                    nodes.Add(passage);
                }

                // 연결 방향 설정
                Direction prev = MapUtils.Vector2IntToDirection(path[i - 1] - pos);
                Direction next = MapUtils.Vector2IntToDirection(path[i + 1] - pos);
                passage.connectDir.Add(prev);
                passage.connectDir.Add(next);
                Connect(pos + MapUtils.Offset[prev], MapUtils.Opposite(prev));
                Connect(pos + MapUtils.Offset[next], MapUtils.Opposite(next));
            }
        }
    }

    void Connect(Vector2Int neighbor, Direction dir)
    {
        MapNode n = nodes.FirstOrDefault(node => node.position == neighbor);

        if (n == null)
            return;

        n.connectDir.Add(dir);
    }

    // 막힌 칸 계산
    HashSet<Vector2Int> BuildBlocked(MapNode a, MapNode b)
    {
        var blocked = new HashSet<Vector2Int>(roomBlocked);

        // 방에 연결될 위치만 뚫어놓기
        foreach (var cell in IterateArea(a.position, a.size.Value, buffer: 1))
            blocked.Remove(cell);
        foreach (var cell in IterateArea(b.position, b.size.Value, buffer: 1))
            blocked.Remove(cell);

        return blocked;
    }



    RoomSizeType GetRandomSize()
    {
        RoomSizeType[] sizes = { RoomSizeType.Small, 
                                RoomSizeType.Medium, RoomSizeType.Medium, RoomSizeType.Medium, 
                                RoomSizeType.Large };
        return sizes[Random.Range(0, sizes.Length)];
    }
}

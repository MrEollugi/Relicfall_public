using System;
using System.Collections.Generic;
using UnityEngine;

public static class MapUtils
{
    public static readonly Direction[] DIR4 = { Direction.Up, Direction.Right,
                                                Direction.Down, Direction.Left };

    public static readonly Dictionary<Direction, Vector2Int> Offset = new()
    {
        { Direction.Up,    Vector2Int.up },
        { Direction.Right, Vector2Int.right },
        { Direction.Down,  Vector2Int.down },
        { Direction.Left,  Vector2Int.left }
    };

    public static Vector2Int[] GetRoomOffsets(RoomSizeType size)
    {
        return size switch
        {
            RoomSizeType.Small => new[] { new Vector2Int(0, 0) },
            RoomSizeType.Medium => new[] {
                new Vector2Int(0,0), new Vector2Int(0,1),
                new Vector2Int(1,0), new Vector2Int(1,1)
            },
            RoomSizeType.Large => new[] {
                new Vector2Int(0,0), new Vector2Int(0,1), new Vector2Int(0,2),
                new Vector2Int(1,0), new Vector2Int(1,1), new Vector2Int(1,2),
                new Vector2Int(2,0), new Vector2Int(2,1), new Vector2Int(2,2)
            },
            _ => new[] { new Vector2Int(0, 0) },
        };
    }

    public static Vector2Int[] GetAreaWithMargin(Vector2Int pos, RoomSizeType size)
    {
        var offsets = GetRoomOffsets(size);
        var margin = new HashSet<Vector2Int>();

        foreach (var offset in offsets)
        {
            var p = pos + offset;
            for (int dx = -1; dx <= 1; dx++)
                for (int dy = -1; dy <= 1; dy++)
                    margin.Add(p + new Vector2Int(dx, dy));
        }
        return new List<Vector2Int>(margin).ToArray();
    }

    public static Vector2Int GetRandomDirection()
    {
        // 위, 아래, 왼, 오 중에 랜덤으로 좌표 get 
        Vector2Int[] dirs = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        return dirs[UnityEngine.Random.Range(0, dirs.Length)];
    }

    public static Vector2Int GetRandomDirectionExclude(Vector2Int exclude)
    {
        Vector2Int[] dirs = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        while (true)
        {
            Vector2Int result = dirs[UnityEngine.Random.Range(0, dirs.Length)];
            if (exclude != result)
                return result;
        }
    }



    // Vector2Int는 논리 좌표로 Unity 월드 좌표로 변환하는 과정 필요
    public static Vector3 GridToWorld(Vector2Int gridPos)
    {
        return new Vector3(gridPos.x * 16f, 0f, gridPos.y * 16f);
    }

    public static Vector2Int WorldToGrid(Vector3 worldPos)
    {
        return new Vector2Int(Mathf.FloorToInt(worldPos.x), Mathf.FloorToInt(worldPos.z));
    }

    // 좌표를 방향으로
    public static Direction Vector2IntToDirection(Vector2Int vec)
    {
        return vec switch
        {
            { x: 0, y: 1 } => Direction.Up,
            { x: 0, y: -1 } => Direction.Down,
            { x: -1, y: 0 } => Direction.Left,
            { x: 1, y: 0 } => Direction.Right,
            _ => throw new ArgumentException($"유효하지 않은 방향 벡터: {vec}")
        };
    }

    // 방향을 좌표로
    public static Vector2Int DirectionToVector2Int(Direction dir)
    {
        return dir switch
        {
            Direction.Up => Vector2Int.up,
            Direction.Down => Vector2Int.down,
            Direction.Left => Vector2Int.left,
            Direction.Right => Vector2Int.right,
            _ => Vector2Int.zero
        };
    }

    // 방향을 벡터로
    public static Vector3 DirectionToVector(Direction dir)
    {
        return dir switch
        {
            Direction.Up => Vector3.up,
            Direction.Down => Vector3.down,
            Direction.Left => Vector3.left,
            Direction.Right => Vector3.right,
            _ => Vector3.zero
        };
    }

    // 반대쪽
    public static Direction Opposite(Direction dir)
    {
        return dir switch
        {
            Direction.Up => Direction.Down,
            Direction.Down => Direction.Up,
            Direction.Left => Direction.Right,
            Direction.Right => Direction.Left,
            _ => dir
        };
    }

    public static Direction DirTo(Vector2Int from, Vector2Int to)
    {
        var d = to - from;
        if (d == Vector2Int.up) return Direction.Up;
        if (d == Vector2Int.down) return Direction.Down;
        if (d == Vector2Int.right) return Direction.Right;
        if (d == Vector2Int.left) return Direction.Left;
        return Direction.None;
    }

    public static int GetSideLength(RoomSizeType size)
    {
        return size switch
        {
            RoomSizeType.Small => 1,
            RoomSizeType.Medium => 2,
            RoomSizeType.Large => 3
        };
    }

    public static Vector3 GetDoorPosition(RoomSizeType size)
    {
        return size switch
        {
            RoomSizeType.Small => new Vector3(13f, 0f, 14.9f),
            RoomSizeType.Medium => new Vector3(20f, 0f, 30.9f),
            RoomSizeType.Large => new Vector3(40f, 0f, 46.9f)
        };
    }
}

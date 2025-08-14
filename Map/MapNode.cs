using System;
using System.Collections.Generic;
using UnityEngine;

public enum RoomSizeType
{
    Small, Medium, Large
}

public enum NodeType 
{ 
    Room, Passage    
}

public enum Direction 
{ 
    Up, Down, Left, Right, None
}

public class MapNode
{
    public NodeType type;
    public Vector2Int position;

    public RoomSizeType? size;

    public HashSet<Direction> connectDir = new();
    // Dictionary<Direction, MapNode> connectInfo = new();

    public event Action<MapNode> OnVisited;

    public bool isVisited;

    public MapNode(NodeType type, Vector2Int position, RoomSizeType? size = null)
    {
        this.type = type;
        this.position = position;
        this.size = size;
        this.isVisited = false;
    }

    public void Visit()
    {
        if (!isVisited)
        {
            isVisited = true;
            OnVisited?.Invoke(this);
        }
    }
}

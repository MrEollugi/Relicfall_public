using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MapGraph
{
    private readonly Dictionary<Vector2Int, MapNode> nodes = new();
    public IEnumerable<MapNode> Nodes => nodes.Values;

    public MapGraph(List<MapNode> nodeList)
    {
        foreach (var n in nodeList)
        {
            this.nodes.Add(n.position, n);
        }
    }

    public MapNode AddRoom(Vector2Int origin, RoomSizeType size)
    {
        var n = new MapNode(NodeType.Room, origin, size);
        nodes[origin] = n; return n;
    }

    public MapNode AddPassage(Vector2Int pos)
    {
        if (nodes.TryGetValue(pos, out var exist)) return exist;
        var n = new MapNode(NodeType.Passage, pos);
        nodes[pos] = n; return n;
    }

    public void Connect(MapNode a, MapNode b, Direction dirAB)
    {
        if (!a.connectDir.Contains(dirAB)) a.connectDir.Add(dirAB);
        var dirBA = MapUtils.Opposite(dirAB);
        if (!b.connectDir.Contains(dirBA)) b.connectDir.Add(dirBA);
    }
}
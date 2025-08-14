using System.Collections.Generic;
using UnityEngine;

public static class Pathfinder
{
    // PriorityQueue Node
    private struct Node : System.IComparable<Node>
    {
        public Vector2Int pos;
        public int gCost;
        public int hCost;
        public int fCost => gCost + hCost;

        public Node(Vector2Int pos, int gCost, int hCost)
        {
            this.pos = pos;
            this.gCost = gCost;
            this.hCost = hCost;
        }

        public int CompareTo(Node other)
        {
            int cmp = fCost.CompareTo(other.fCost);
            if (cmp != 0) return cmp;

            cmp = hCost.CompareTo(other.hCost);
            if (cmp != 0) return cmp;

            cmp = pos.x.CompareTo(other.pos.x);
            return cmp != 0 ? cmp : pos.y.CompareTo(other.pos.y);
        }
    }

    // A* 알고리즘
    public static List<Vector2Int>? AStar(Vector2Int start, Vector2Int goal,
                                        HashSet<Vector2Int> blocked,
                                        System.Func<Vector2Int, bool>? outOfBounds = null)     
    {
        var open = new PriorityQueue<Node>();
        var gScore = new Dictionary<Vector2Int, int>();
        var cameFrom = new Dictionary<Vector2Int, Vector2Int>();

        gScore[start] = 0;
        open.Enqueue(new Node(start, 0, Manhattan(start, goal)));

        while (open.Count > 0)
        {
            var current = open.Dequeue();
            if (current.gCost != gScore[current.pos]) continue;

            if (current.pos == goal)
                return ReconstructPath(goal, cameFrom);

            foreach (var dir in MapUtils.DIR4)
            {
                var neighbor = current.pos + MapUtils.Offset[dir];
                if (blocked.Contains(neighbor)) continue;
                if (outOfBounds != null && outOfBounds(neighbor)) continue;

                int tentativeG = gScore[current.pos] + 1;

                if (gScore.TryGetValue(neighbor, out int prevG) && tentativeG >= prevG)
                    continue;

                gScore[neighbor] = tentativeG;
                cameFrom[neighbor] = current.pos;

                int h = Manhattan(neighbor, goal);
                open.Enqueue(new Node(neighbor, tentativeG, h));
            }
        }

        return null;
    }

    private static int Manhattan(Vector2Int a, Vector2Int b)
        => Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);

    private static List<Vector2Int> ReconstructPath(Vector2Int end, Dictionary<Vector2Int, Vector2Int> came)
    {
        var path = new List<Vector2Int> { end };
        while (came.TryGetValue(end, out var prev))
        {
            path.Add(prev);
            end = prev;
        }
        path.Reverse();
        return path;
    }
}

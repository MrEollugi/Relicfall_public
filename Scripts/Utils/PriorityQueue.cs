using System;
using System.Collections.Generic;

public class PriorityQueue<T> where T : IComparable<T>
{
    private readonly List<T> _heap = new();

    public int Count => _heap.Count;

    public void Enqueue(T item)
    {
        _heap.Add(item);
        HeapUp(_heap.Count - 1);
    }

    public T Dequeue()
    {
        if (_heap.Count == 0)
            throw new InvalidOperationException("Queue is empty.");

        T root = _heap[0];
        int last = _heap.Count - 1;
        _heap[0] = _heap[last];
        _heap.RemoveAt(last);

        if (_heap.Count > 0)
            HeapDown(0);

        return root;
    }

    public bool TryDequeue(out T item)
    {
        if (_heap.Count == 0)
        {
            item = default;
            return false;
        }

        item = Dequeue();
        return true;
    }

    public T Peek() => _heap[0];

    private void HeapUp(int i)
    {
        while (i > 0)
        {
            int parent = (i - 1) / 2;
            if (_heap[i].CompareTo(_heap[parent]) >= 0) break;
            (_heap[i], _heap[parent]) = (_heap[parent], _heap[i]);
            i = parent;
        }
    }

    private void HeapDown(int i)
    {
        int last = _heap.Count - 1;
        while (true)
        {
            int left = 2 * i + 1;
            if (left > last) break;

            int right = left + 1;
            int smallest = (right <= last && _heap[right].CompareTo(_heap[left]) < 0) ? right : left;

            if (_heap[i].CompareTo(_heap[smallest]) <= 0) break;
            (_heap[i], _heap[smallest]) = (_heap[smallest], _heap[i]);
            i = smallest;
        }
    }
}

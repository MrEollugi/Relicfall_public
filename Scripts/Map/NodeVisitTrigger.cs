using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class NodeVisitTrigger : MonoBehaviour
{
    private MapNode mapNode;
    private MapManager mapManager;

    public void Initialize(MapNode node, MapManager manager)
    {
        mapNode = node;
        mapManager = manager;

        var col = GetComponent<Collider>();
        if (col == null)
        {
            col = gameObject.AddComponent<BoxCollider>();
        }
        col.isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            mapManager.NotifyNodeVisited(mapNode);
        }
    }
}

using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshSurface))]
public class MapNavMeshBaker : MonoBehaviour
{
    private NavMeshSurface surface;

    private void Awake()
    {
        surface = GetComponent<NavMeshSurface>();
    }

    public void Bake()
    {
        surface.BuildNavMesh();
    }
}

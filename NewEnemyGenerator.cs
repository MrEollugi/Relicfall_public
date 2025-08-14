using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NewEnemyGenerator : MonoBehaviour
{
    [System.Serializable]
    public class EnemyEntry
    {
        public EnemyDataSO enemyData;
        public int spawnWeight = 1; // 가중치
    }

    [SerializeField] private List<EnemyEntry> enemyPrefabs; 

    public int enemyCount;
    public GameObject spawnPoints;
    public float spawnRadius;
    private int lostSpawnCount;
    public Transform enemyParent;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(0.1f);

        if (enemyParent == null)
            enemyParent = new GameObject("EnemyParent").transform;


        var triangulation = NavMesh.CalculateTriangulation();

        bool navMeshExists = triangulation.vertices != null &&
                             triangulation.vertices.Length > 0 &&
                             triangulation.indices != null &&
                             triangulation.indices.Length > 0;
        if (!navMeshExists)
        {
            Debug.LogError("NavMesh가 존재하지 않습니다");
        }


        for (int i = 0; i < enemyCount; i++)
        {
            Create(GetRandomPointOnNavMesh(), Quaternion.identity, enemyParent);
        }
    }
    private void Update()
    {
        if(lostSpawnCount != 0)
        {
            for (int i = 0; i < lostSpawnCount; i++)
            {
                Create(GetRandomPointOnNavMesh(), Quaternion.identity, enemyParent);
            }
            lostSpawnCount = 0;
        }
    }

    private bool IsInSpawn(Vector3 position)
    {
        var spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");

        foreach (var sp in spawnPoints)
        {
            float distance = Vector3.Distance(sp.transform.position, position);

            if (distance <= spawnRadius)
            {
                return true;
            }
        }
        return false;
    }
    public static Vector3 GetRandomPointOnNavMesh()
    {
        var navMesh = NavMesh.CalculateTriangulation();   
        int triIndex = Random.Range(0, navMesh.indices.Length / 3) * 3; 

        Vector3 a = navMesh.vertices[navMesh.indices[triIndex]];
        Vector3 b = navMesh.vertices[navMesh.indices[triIndex + 1]];
        Vector3 c = navMesh.vertices[navMesh.indices[triIndex + 2]];

        float r1 = Mathf.Sqrt(Random.value);
        float r2 = Random.value;
        Vector3 point = (1 - r1) * a + r1 * (1 - r2) * b + r1 * r2 * c;

        return point; 
    }

    // 실제 오브젝트 생성 함수
    public GameObject Create(Vector3 pos, Quaternion rot, Transform parent)
    {
        if(IsInSpawn(pos) == false)
        {
            GameObject enemyPrefab = GetRandomEnemyPrefab();
            if (enemyPrefab == null)
            {
                Debug.LogWarning($"Create() {enemyPrefab} 없음");
                return null;
            }


            Instantiate(enemyPrefab, pos, rot, parent);

            return enemyPrefab;
        }
        else
        {
            lostSpawnCount++;

            return null;
        }

    }

    private GameObject GetRandomEnemyPrefab()
    {
        if (enemyPrefabs == null)
            return null;

        // 전체 가중치 합산
        int totalWeight = 0;
        foreach (var mob in enemyPrefabs)
            totalWeight += mob.spawnWeight;

        int rand = Random.Range(0, totalWeight);

        int current = 0;

        foreach (var entry in enemyPrefabs)
        {
            current += entry.spawnWeight;
            if (rand < current)
                return entry.enemyData.enemyPrefabs;
        }

        return null;
    }
}



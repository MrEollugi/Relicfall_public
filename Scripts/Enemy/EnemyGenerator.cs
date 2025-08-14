using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class EnemyEntry
{
    public EnemyDataSO enemyData;
    public int spawnWeight = 1; // 가중치
}


public class EnemyGenerator : MonoBehaviour
{

    [Header("몬스터 목록")]
    [SerializeField] private EnemyEntry[] enemyEntries;

    [SerializeField] private int maxAttempts = 1000;    //무한 루프 방지용
    public float dungeonRadius;
    public int enemyCount;

    public int spawnRadius;

    private IEnumerator Start()
    {
        
        // NavMesh가 생성된 이후 아이템 생성 
        yield return new WaitForSeconds(0.1f);
        GenerateEnemysOnNavMesh(transform.position, dungeonRadius, enemyCount);
    }

    private bool IsInSpawn(Vector3 position)
    {
        var spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");

        foreach (var sp in spawnPoints)
        {
            // 각 SpawnPoint를 중심으로 거리 체크
            float distance = Vector3.Distance(sp.transform.position, position);

            if (distance <= spawnRadius)
            {
                // 하나라도 안에 있으면 true 반환
                return true;
            }
        } 
        return false;
    }

    public void GenerateEnemysOnNavMesh(Vector3 center, float radius, int count)
    {
        int spawned = 0;
        int attempts = 0;



        while (spawned < count && attempts < maxAttempts)
        {
            Vector3 randomPos = center + Random.insideUnitSphere * radius;
            randomPos.y = center.y;

            // Debug.Log($"시도 {attempts}번째, spawned {spawned}개, pos: {randomPos}");

            if (NavMesh.SamplePosition(randomPos, out NavMeshHit hit, 0.5f, NavMesh.AllAreas))
            {

                // 주변에 장애물이 있는지 확인
                //bool blockedNear = false;

                //foreach(var collider in Physics.OverlapSphere(hit.position, 0.3f))
                //{
                //    if (collider.CompareTag("Wall"))
                //    {
                //        blockedNear = true;
                //        break;
                //    }
                //}

                    
                    
                 // 위쪽에 장애물이 있는지도 확인
                bool blockedAbove = Physics.Raycast(hit.position + Vector3.up * 0.1f, Vector3.up, 1f);

                bool isSpawn = IsInSpawn(randomPos);
                // Debug.Log($"NavMesh 위치 OK / blockedNear: , blockedAbove: {blockedAbove}, pos: {hit.position}");

                if (/*!blockedNear &&*/ !blockedAbove && isSpawn == false)
                {

                    GameObject enemyPrefab = GetRandomEnemyPrefab();

                    // Debug.Log($"==> 몬스터 소환 준비, 프리팹: {(enemyPrefab != null ? enemyPrefab.name : "null")}");
                    if (enemyPrefab != null)
                    {
                        // Debug.Log("Enemy Prefab: " + enemyPrefab.name);
                        Instantiate(enemyPrefab, hit.position + Vector3.up * 0.1f, Quaternion.identity);
                        // Debug.Log($"===>> 몬스터 {enemyPrefab.name} 생성됨, pos: {hit.position} 몬스터 : {spawned + 1}마리 소환됨");
                        spawned++;
                    }
                    else
                    {
                        // Debug.LogWarning("enemyPrefab is null! EnemyEntry or EnemyDataSO 연결 확인");
                    }
                }
            } 

            attempts++;
        }
    }

    private GameObject GetRandomEnemyPrefab()
    {
        if (enemyEntries == null || enemyEntries.Length == 0)
            return null;

        // 전체 가중치 합산
        int totalWeight = 0;
        foreach (var mob in enemyEntries)
            totalWeight += mob.spawnWeight;

        int rand = Random.Range(0, totalWeight);

        // 가중치 누적값
        int current = 0;

        // 각 아이템의 가중치를 누적하면서 선택된 가중치 범위에 해당하는 아이템 반환
        foreach (var entry in enemyEntries)
        {
            current += entry.spawnWeight;
            if (rand < current)
                return entry.enemyData.enemyPrefabs;//몬스터 소환할 프리펩 만들어야함
        }

        return null;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        // 1. 전체 소환 범위(파란색)
        Gizmos.color = new Color(0, 0.5f, 1f, 0.3f); // 반투명 파랑
        Gizmos.DrawWireSphere(transform.position, dungeonRadius);

        // 2. 모든 SpawnPoint의 금지 반경(빨간색)
        Gizmos.color = new Color(1f, 0.1f, 0.1f, 0.3f);
        var spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
        foreach (var sp in spawnPoints)
        {
            Gizmos.DrawWireSphere(sp.transform.position, spawnRadius);
        }
    }
#endif

}


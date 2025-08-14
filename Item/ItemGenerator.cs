using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ItemEntry
{
    public string itemId;
    public int spawnWeight = 1; // 아이템 가중치

    [HideInInspector] public ItemType type; // id를 통해 캐싱
}

public class ItemGenerator : MonoBehaviour
{
    [Header("아이템 타입별 비율")]
    [SerializeField] private float curiosRatio = 7.5f;
    [SerializeField] private float relicRatio = 1f;
    [SerializeField] private float consumableRatio = 1.5f;

    [Header("스폰 설정")]
    [SerializeField] private int maxAttempts = 1000;
    public int itemCount;

    [SerializeField] private LayerMask blockMask; // 벽, 장애물 등만 포함
    [SerializeField] private float nearRadius = 0.15f; // 주변 충돌 반경
    [SerializeField] private float aboveClearance = 0.5f; // 위로 여유 높이
    [SerializeField] private float minSpacing = 1.2f; // 아이템 간 최소 거리

    private Dictionary<ItemType, List<ItemEntry>> itemsByType;
    private Dictionary<ItemType, int> totalWeightByType;

    private readonly List<Vector3> spawnedPositions = new();

    private void Awake()
    {
        #region 생성할 아이템 데이터 캐싱
        var table = new (string id, int weight)[]
        {
            ("curios_empty_bottle", 8),
            ("curios_iron_goblet", 7),
            ("curios_plain_broom", 6),
            ("curios_unidentified_scroll", 6),
            ("curios_ominous_skeleton_key", 5),
            ("curios_ancient_grimoire", 4),
            ("curios_chronicles_of_the_forgotten", 4),
            ("curios_ancient_black_tome", 3),
            ("curios_arcane_codex", 3),
            ("curios_emerald_binding", 3),
            ("curios_copper_ore", 4),
            ("curios_silver_ore", 3),
            ("curios_iron_ore", 3),
            ("curios_gold_ore", 2),

            ("relic_butchers_cleaver", 6),
            ("relic_crude_dagger", 6),
            ("relic_worn_shortsword", 5),
            ("relic_knights_sword", 3),
            ("relic_greatsword_of_resolve", 1),
            ("relic_splintered_buckler", 4),
            ("relic_wooden_kite_shield", 3),
            ("relic_reinforced_round_shield", 1),
            ("relic_grave_diggers_shovel", 2),
            ("relic_cursed_skull", 1),

            ("consumable_lesser_potion_healing", 7),
            ("consumable_lesser_potion_mana", 7),
            ("consumable_lesser_potion_stamina", 7),
            ("consumable_medium_potion_healing", 3),
            ("consumable_medium_potion_mana", 3),
            ("consumable_fire_torch", 5),
        };

        itemsByType = new Dictionary<ItemType, List<ItemEntry>>();
        totalWeightByType = new Dictionary<ItemType, int>();

        foreach (ItemType type in System.Enum.GetValues(typeof(ItemType)))
        {
            itemsByType[type] = new List<ItemEntry>();
            totalWeightByType[type] = 0;
        }

        foreach (var (id, weight) in table)
        {
            var data = ItemDatabase.Instance.GetItemById(id);
            if (data == null || !System.Enum.TryParse<ItemType>(data.type, out var parsedType))
            {
                Debug.LogWarning($"{id} ItemType 파싱 실패");
                continue;
            }

            var entry = new ItemEntry
            {
                itemId = id,
                spawnWeight = weight,
                type = parsedType
            };

            itemsByType[parsedType].Add(entry);
            totalWeightByType[parsedType] += weight;
        }
        #endregion
    }

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(0.1f);
        yield return GenerateItemsOnNavMesh(transform.position, itemCount);
    }

    #region 네브매쉬 위 랜덤 위치 가져오기
    public static Vector3 GetRandomPointOnNavMesh()
    {
        var navMeshData = NavMesh.CalculateTriangulation();
        if (navMeshData.indices.Length == 0) return Vector3.zero;

        int tri = Random.Range(0, navMeshData.indices.Length / 3);
        Vector3 v0 = navMeshData.vertices[navMeshData.indices[tri * 3 + 0]];
        Vector3 v1 = navMeshData.vertices[navMeshData.indices[tri * 3 + 1]];
        Vector3 v2 = navMeshData.vertices[navMeshData.indices[tri * 3 + 2]];

        float a = Random.value;
        float b = Random.value;
        if (a + b > 1f)
        {
            a = 1f - a;
            b = 1f - b;
        }
        float c = 1f - a - b;
        return v0 * a + v1 * b + v2 * c;
    }
    #endregion

    #region 아이템 생성
    public async Task GenerateItemsOnNavMesh(Vector3 center, int count)
    {
        int spawned = 0;
        int attempts = 0;

        while (spawned < count && attempts < maxAttempts)
        {
            Vector3 spawnPos = GetRandomPointOnNavMesh();
            spawnPos.y = center.y;

            if (NavMesh.SamplePosition(spawnPos, out NavMeshHit hit, 0.5f, NavMesh.AllAreas))
            {
                if (CanPlace(hit.position))
                {
                    var item = GetRandomItemData();
                    if (item != null)
                    {
                        await SpawnAddressablePrefab(item.dropPrefabAddress, hit.position + Vector3.up * 0.1f, item.id);
                        spawnedPositions.Add(hit.position);
                        spawned++;
                    }
                }
            }
            attempts++;
        }
    }
    #endregion

    #region 배치 가능 여부 판정
    private bool CanPlace(Vector3 pos)
    {
        // 주변 충돌 체크
        if (Physics.CheckSphere(pos, nearRadius, blockMask, QueryTriggerInteraction.Ignore))
            return false;

        // 위쪽 여유 체크
        if (Physics.Raycast(pos + Vector3.up * 0.05f, Vector3.up, aboveClearance, blockMask, QueryTriggerInteraction.Ignore))
            return false;

        // 아이템끼리 너무 가까운지 체크
        foreach (var p in spawnedPositions)
        {
            if ((p - pos).sqrMagnitude < minSpacing * minSpacing)
                return false;
        }

        return true;
    }
    #endregion

    #region 랜덤 ItemData 얻기
    private ItemData GetRandomItemData()
    {
        float totalRatio = curiosRatio + relicRatio + consumableRatio;
        float roll = Random.value * totalRatio;
        ItemType chosenType = (roll < curiosRatio)
            ? ItemType.Curios
            : (roll < curiosRatio + relicRatio) ? ItemType.Relic : ItemType.Consumable;

        var list = itemsByType[chosenType];
        if (list.Count == 0) return null;

        int totalWeight = totalWeightByType[chosenType];
        int rand = Random.Range(0, totalWeight);
        int current = 0;

        foreach (var entry in list)
        {
            current += entry.spawnWeight;
            if (rand < current)
                return ItemDatabase.Instance.GetItemById(entry.itemId);
        }
        return null;
    }
    #endregion

    #region Addressables 프리팹 로드 & 스폰
    private async Task SpawnAddressablePrefab(string address, Vector3 position, string itemId = null)
    {
        GameObject prefab = await ItemAddressable.LoadPrefabAsync(address);

        if (prefab != null)
        {
            Quaternion rot = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f) * prefab.transform.localRotation;

            GameObject go = Instantiate(prefab, position, rot);
            if (itemId != null)
            {
                var item = go.GetComponent<Item>();
                if (item != null)
                    item.ItemId = itemId;
            }
        }
    }
    #endregion
}

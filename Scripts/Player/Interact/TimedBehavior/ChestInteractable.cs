using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ChestInteractable : MonoBehaviour, ITimedInteractable, IWorldTooltipDataProvider
{
    [SerializeField] private float interactionTime = 1f;
    public float InteractionTime => interactionTime;

    [SerializeField] private StorageType chestType;

    [SerializeField] private string chestUIAddress = "ChestUI"; // 어드레서블 주소

    private static ChestUI openedUI = null;
    public static ChestUI OpenedUI => openedUI;

    private InventoryGrid _chestGrid;

    private void Start()
    {
        // 최초 생성 시 세이브 데이터에서 불러오거나 새로 만듦
        _chestGrid = LoadOrCreateChestGrid();
    }

    public void OnInteractionComplete(PlayerController player)
    {
        GameManager.Instance.CurrentPlayer.wasChestJustClosed = false;

        if (openedUI != null)
        {
            if (!openedUI.gameObject.activeSelf)
            {
                openedUI.gameObject.SetActive(true);
                openedUI.Init(player, _chestGrid);
                player.BlockInput();
            }
            return;
        }

        player.BlockInput();

        Addressables.LoadAssetAsync<GameObject>(chestUIAddress).Completed += (handle) =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                var chestUIObj = Instantiate(handle.Result);
                var chestUI = chestUIObj.GetComponent<ChestUI>();
                chestUI.Init(player, _chestGrid);

                chestUIObj.SetActive(true);

                openedUI = chestUI;

                chestUI.OnClosed += () =>
                {
                    SaveChestGrid(_chestGrid);
                    player.UnblockInput();
                    //Addressables.Release(handle);
                };
            }
            else
            {
                Debug.LogError($"[ChestInteractable] Addressable {chestUIAddress} 로드 실패! Address를 확인하세요.");
                player.UnblockInput();
            }
        };

    }
    private InventoryGrid LoadOrCreateChestGrid()
    {
        var worldData = GameManager.Instance.WorldData;
        string key = chestType == StorageType.Hub ? "hub" : "wagon";

        if (worldData.Storages == null)
            worldData.Storages = new Dictionary<string, List<ItemInstanceSaveData>>();

        if (!worldData.Storages.TryGetValue(key, out var itemList) || itemList == null)
        {
            worldData.Storages[key] = new List<ItemInstanceSaveData>();
            return new InventoryGrid(5, 5, false);
        }
        else
        {
            var grid = new InventoryGrid(5, 5, false);
            grid.LoadFromList(itemList);
            return grid;
        }
    }

    public void OnInteractionCanceled()
    {
        // 예: 사운드 끄기, UI 닫기 등
    }

    private InventoryGrid GetOrCreateChestGrid()
    {
        var worldData = GameManager.Instance.WorldData;
        string key = chestType == StorageType.Hub ? "hub" : "wagon";

        if (worldData.Storages == null)
            worldData.Storages = new Dictionary<string, List<ItemInstanceSaveData>>();

        if (!worldData.Storages.TryGetValue(key, out var itemList) || itemList == null)
        {
            var grid = new InventoryGrid(5, 5, false);
            worldData.Storages[key] = new List<ItemInstanceSaveData>();
            return grid;
        }
        else
        {
            var grid = new InventoryGrid(5, 5, false);
            grid.LoadFromList(itemList);
            return grid;
        }
    }

    private void SaveChestGrid(InventoryGrid grid)
    {
        var worldData = GameManager.Instance.WorldData;
        string key = chestType == StorageType.Hub ? "hub" : "wagon";

        if (worldData.Storages == null)
            worldData.Storages = new Dictionary<string, List<ItemInstanceSaveData>>();

        worldData.Storages[key] = grid.GetAllItems().Select(i => i.ToSaveData()).ToList();
    }

    public TooltipType GetTooltipType()
    {
        return TooltipType.Chest;
    }

    public void InitializeTooltip(GameObject tooltipInstance)
    {
    }
}

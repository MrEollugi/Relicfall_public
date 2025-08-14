using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugManager;

public enum InventoryGridViewMode
{
    Player,
    Chest
}

public class InventoryUI : MonoBehaviour
{
    //[SerializeField] private GameObject MainUIWindow;

    [SerializeField] private GameObject inventoryWindow;
    [SerializeField] private GameObject itemInfoWindow;
    [SerializeField] private GameObject relicSlotWindow;

    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private Transform slotParent;
    [SerializeField] private Transform relicSlotParent;

    [SerializeField] private int addSlotCount = 2;      // 추가될 슬롯 개수
    public int AddSlotCount { get => addSlotCount; set => addSlotCount = value; }

    private int totalSlots;
    private PlayerController player;
    public PlayerController Player { get => player; }
    private GridLayoutGroup gridLayoutGroup;
    private SlotUI[,] slotUIs;
    private InventoryGrid grid;
    public InventoryGrid Grid { get => grid; }
    private ItemInfoUI itemInfoUI;
    private RelicSlotUI relicSlotUI;
    private QuickSlotPanelUI quickSlotUI;
    public RelicSlotUI RelicSlotUI { get => relicSlotUI; }
    public QuickSlotPanelUI QuickSlotPanelUI { get => quickSlotUI; }
    private PopUpUI popUp;
    private SlotUI[] relicSlotUIs;

    public bool IsOpen => inventoryWindow.activeInHierarchy;

    private InventoryGridViewMode mode;

    #region Unity Init

    private void Awake()
    {
        player = FindObjectOfType<PlayerController>();
        gridLayoutGroup = GetComponentInChildren<GridLayoutGroup>();

        if (itemInfoWindow != null)
            itemInfoUI = itemInfoWindow.GetComponent<ItemInfoUI>();
        if (relicSlotWindow != null)
            relicSlotUI = relicSlotWindow.GetComponent<RelicSlotUI>();
        if (relicSlotWindow != null)
            relicSlotUIs = relicSlotParent.GetComponentsInChildren<SlotUI>();
        if (relicSlotUIs != null)
        {
            foreach (var slot in relicSlotUIs)
            {
                slot.SetParentInventoryUI(this);
                slot.SetRelicSlot(true); // 렐릭 전용 동작
            }
        }

        var quickSlotPanel = GameObject.FindObjectOfType<QuickSlotPanelUI>();
        if (quickSlotPanel != null)
        {
            quickSlotUI = quickSlotPanel;
            foreach (var slot in quickSlotUI.QuickSlots)
            {
                slot.SetParentInventoryUI(this);
                slot.SetRelicSlot(false); // 퀵슬롯은 유물 아님
                slot.SetViewMode(InventoryGridViewMode.Player);
            }
        }

        popUp = GetComponent<PopUpUI>();
        //InventoryManager.Instance.InventoryUI = this;
    }

    private void Start()
    {
        //itemInfoUI = itemInfoWindow.GetComponent<ItemInfoUI>();
        //relicSlotUI = relicSlotWindow.GetComponent<RelicSlotUI>();
        //popUp = GetComponent<PopUpUI>();

    }

    #endregion

    public void SetConstraintCount(int width)
    {
        gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayoutGroup.constraintCount = width;
    }

    #region 인벤토리 초기화
    public void Init(int width, int height, InventoryGrid inventoryGrid, InventoryGridViewMode mode = InventoryGridViewMode.Player, bool isViewingChestInventory = false)
    {
        this.mode = mode;
        totalSlots = width * height;
        inventoryGrid.IsViewingChestInventory = isViewingChestInventory;

        if (grid != null)
        {
            grid.OnInventoryChanged -= UpdateUI;
        }

        for (int i = slotParent.childCount - 1; i >= 0; i--)
        {
            Destroy(slotParent.GetChild(i).gameObject);
        }

        // slotUIs = null;
        // Debug.Log($"[InventoryUI] Init: grid ref = {inventoryGrid.GetHashCode()}");
        grid = inventoryGrid;
        SetConstraintCount(width);
        slotUIs = new SlotUI[width, height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                GameObject slotObj = Instantiate(slotPrefab, slotParent);
                SlotUI slot = slotObj.GetComponentInChildren<SlotUI>();
                slotUIs[x, y] = slot;
                slot.Init(x, y);
                slot.ClearSlot();
                slot.SetParentInventoryUI(this);
                slot.SetViewMode(mode);
            }
        }

        relicSlotUI?.Init(this);
        quickSlotUI?.UpdateUI();

        switch (mode)
        {
            case InventoryGridViewMode.Player:
                inventoryWindow.SetActive(false);    // B키로 열기 전엔 닫혀있음
                relicSlotWindow.SetActive(true);     // 플레이어 인벤토리에서 Relic 항상 표시
                break;
            case InventoryGridViewMode.Chest:
                inventoryWindow.SetActive(true);     // ChestUI 열릴 때는 항상 활성화
                relicSlotWindow.SetActive(true);     // 창고에서도 보여줄 경우 true
                break;
        }
        if (itemInfoWindow != null)
            itemInfoWindow.SetActive(false);

        grid.OnInventoryChanged += UpdateUI;
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (grid == null) return;

        if (slotUIs == null || slotUIs.GetLength(0) != grid.Width || slotUIs.GetLength(1) != grid.Height)
        {
            return; 
        }

        for (int x = 0; x < grid.Width; x++)
        {
            for (int y = 0; y < grid.Height; y++)
            {
                ItemInstance item = grid.GetItemAt(x, y);
                slotUIs[x, y]?.SetItem(item);
                //if (item != null)
                //    Debug.Log($"[UI] item at ({x},{y}) = {item.data.id} anchor=({item.AnchorX},{item.AnchorY})");

                slotUIs[x, y]?.UpdateBlockedSlots();
            }
        }

        quickSlotUI?.UpdateUI();
    }

    public void SetGrid(InventoryGrid grid)
    {
        this.grid.OnInventoryChanged -= UpdateUI;
        this.grid = grid;
        grid.OnInventoryChanged += UpdateUI;
    }
    #endregion

    #region 슬롯 추가
    public void AddSlots()
    {
        int currentWidth = grid.Width;
        int currentHeight = grid.Height;

        totalSlots += addSlotCount;
        Debug.Log(totalSlots);

        int newWidth = currentWidth;
        int newHeight = Mathf.CeilToInt((float)totalSlots / currentWidth);

        grid.ResizeGrid(newWidth, newHeight);

        grid.OnInventoryChanged -= UpdateUI;

        for (int i = slotParent.childCount - 1; i >= 0; i--)
        {
            Destroy(slotParent.GetChild(i).gameObject);
        }

        slotUIs = new SlotUI[newWidth, newHeight];

        SetConstraintCount(newWidth);

        for (int y = 0; y < newHeight; y++)
        {
            for (int x = 0; x < newWidth; x++)
            {
                int index = y * newWidth + x;
                if (index < totalSlots)
                {
                    CreateSlot(x, y);
                }
            }
        }

        grid.OnInventoryChanged += UpdateUI;
        UpdateUI();
    }

    private void CreateSlot(int x, int y)
    {
        GameObject slotObj = Instantiate(slotPrefab, slotParent);
        SlotUI slot = slotObj.GetComponentInChildren<SlotUI>();
        slotUIs[x, y] = slot;
        slot.Init(x, y);
        slot.ClearSlot();
    }
    #endregion

    #region 슬롯 사용 불가
    public void SetBlockSlots(int count)
    {
        if (count <= 0) return;

        int blockCount = Mathf.Min(count, totalSlots);
        for (int i = 0; i < blockCount; i++)
        {
            ItemInstance item = grid.BlockRandomCells();
            if (item != null)
            {
                ThrowItem(item, item.stackCount);
            }
        }
    }

    public void UnblockSlots()
    {
        grid.UnblockCells();
    }
    #endregion

    #region 슬롯 관련
    public bool HasSlot(int x, int y)
    {
        if (slotUIs == null)
        {
            return false;
        }

        if (x < 0 || x >= slotUIs.GetLength(0) || y < 0 || y >= slotUIs.GetLength(1))
        {
            return false;
        }

        if (slotUIs[x, y] == null)
            return false;

        return true;
    }

    public void DetectSlots(bool isDetecting)
    {
        int w = Grid.Width;
        int h = Grid.Height;

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                if (HasSlot(x, y))
                {
                    var canvas = slotUIs[x, y].detectCanvas;
                    if (canvas != null)
                        canvas.sortingOrder = isDetecting ? 15 : 0;
                }
            }
        }
    }
    #endregion

    #region 아이템 버리기
    public void ThrowItem(ItemInstance item, int count)
    {
        if (item == null || count <= 0) return;

        ItemInstance dropInstance;
        
        if (item.stackCount > count)
        {
            // 인벤토리에서 일부 수량만 감소
            item.stackCount -= count;

            dropInstance = new ItemInstance(item.data)
            {
                stackCount = count
            };
        }
        else
        {
            // 버리게 될 아이템 인스턴스
            dropInstance = new ItemInstance(item.data)
            {
                stackCount = count
            };

            grid.RemoveItem(item);

            item.stackCount = 0;    // 해당 아이템이 없음을 의미
        }
        
        UpdateUI();
        relicSlotUI.UpdateUI();
        quickSlotUI?.UpdateUI();

        if (player == null) return;

        if (dropInstance.data.type == "Relic")
        {
            if (player.IsEquipped(item))
            {
                player.UnEquipItem(item);
            }
        }

        Vector3 playerPos = player.transform.position;
        _ = DropItemWithAddressables(dropInstance, player.transform.position);
    }

    public async Task DropItemWithAddressables(ItemInstance dropInstance, Vector3 playerPos)
    {
        string address = dropInstance.data.dropPrefabAddress;
        GameObject prefab = await ItemAddressable.LoadPrefabAsync(address);

        if (prefab == null) return;

        Vector3 dropOffset = GetRandomDropOffset();
        Vector3 dropPos = playerPos + dropOffset;

        Quaternion rot = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f) * prefab.transform.localRotation;

        GameObject dropObj = Instantiate(prefab, dropPos, rot);

        // 아이템 프리팹에 데이터 세팅
        if (dropObj.TryGetComponent<Item>(out var itemComponent))
            itemComponent.Init(dropInstance);
    }

    private Vector3 GetRandomDropOffset(float minRadius = 1f, float maxRadius = 2f)
    {
        float angle = Random.Range(0f, Mathf.PI * 2f);
        float radius = Random.Range(minRadius, maxRadius);

        float x = Mathf.Cos(angle) * radius;
        float z = Mathf.Sin(angle) * radius;

        return new Vector3(x, 0f, z);
    }
    #endregion

    #region 인벤토리UI 토글
    //public void Toggle(InventoryGrid inventoryGrid)
    //{
    //    // 새 그리드가 들어오면 UI를 초기화
    //    if (Grid != inventoryGrid)
    //        Init(inventoryGrid.Width, inventoryGrid.Height, inventoryGrid);

    //    // 실제 토글 로직 호출 (기존 파라미터 없는 Toggle)
    //    Toggle();
    //}

    public void Toggle()
    {
        if (IsOpen)
        {
            //MainUIWindow.SetActive(true);
            inventoryWindow.SetActive(false);
            relicSlotWindow.SetActive(false);
        }
        else
        {
            //MainUIWindow.SetActive(false);
            inventoryWindow.SetActive(true);
            relicSlotWindow.SetActive(true);

            popUp?.Show();
            relicSlotUI?.PopUp();
            quickSlotUI.PopUp();

            if (quickSlotUI != null)
            {
                quickSlotUI.SyncQuickSlots();
                quickSlotUI.PopUp();
            }
        }
        if (!IsOpen) UpdateUI();
    }
    #endregion

    #region 인벤토리 내 아이템 정보 표시
    public void SetItemInfo(ItemInstance item)
    {
        if (itemInfoWindow != null)
        {
            itemInfoWindow.SetActive(true);
            itemInfoUI.SetInfo(item);
        }
    }

    public void ClearItemInfo()
    {
        if (itemInfoWindow != null)
        {
            itemInfoWindow.SetActive(false);
            itemInfoUI.ClearInfo();
        }
    }
    #endregion

    public bool IsPlayerMode() => mode == InventoryGridViewMode.Player;
    public bool IsChestMode() => mode == InventoryGridViewMode.Chest;
}

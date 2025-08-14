using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum SlotType
{
    Normal, Relic, Quick, Chest
}

public class SlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    private Image slotImage;
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI itemCountText;
    [SerializeField] public SlotType slotType;

    public Canvas detectCanvas;
    public Color baseColor = new Color32(118, 118, 118, 255);
    public Color blockedTintColor = new Color32(170, 51, 51, 255);
    public Image blockImage;

    private bool isDragging = false;
    private bool isCursorEnter = false;
    [SerializeField] private bool isRelicSlot = false;
    [SerializeField] private bool isQuickSlot = false;
    public void SetQuickSlot(bool value) => isQuickSlot = value;

    public int QuickSlotIndex { get; private set; } = -1;
    public void SetQuickSlotIndex(int idx) => QuickSlotIndex = idx;

    private ItemInstance item;
    public ItemInstance Item { get => item; set => item = value; }

    private RectTransform rt;

    private SlotHighlight highlight;

    public int GridX { get; private set; }
    public int GridY { get; private set; }

    private InventoryUI parentInventoryUI;

    private InventoryGridViewMode viewMode;
    public InventoryUI ParentInventoryUI => parentInventoryUI;
    public InventoryGridViewMode ViewMode => viewMode;

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    #region 슬롯 초기화
    public void Init(int x, int y)
    {
        slotImage = GetComponent<Image>();

        if (detectCanvas == null)
            detectCanvas = transform.Find("detectSlot")?.GetComponent<Canvas>();

        rt = GetComponent<RectTransform>();

        if (slotType == SlotType.Normal || slotType == SlotType.Relic || slotType == SlotType.Chest)
        {
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(0, 1);
            rt.pivot = new Vector2(0, 1);
        }
        else if (slotType == SlotType.Quick)
        {
        }

        highlight = GetComponent<SlotHighlight>();

        GridX = x;
        GridY = y;

        if (slotType == SlotType.Normal && !isRelicSlot)
            UpdateBlockedSlots();
    }

    public void SetRelicSlot(bool value)
    {
        isRelicSlot = value;
    }
    #endregion

    #region 슬롯 아이템 정보 초기화
    public void SetItem(ItemInstance itemInstance)
    {
        bool isViewingChestInventory = IsInChestUI();

        item = itemInstance;

        if (itemInstance == null || itemInstance.stackCount <= 0)
        {
            ClearSlot();
            return;
        }

        if (item.IsStackable)
        {
            itemCountText.text = item.stackCount.ToString();
        }
        else
        {
            itemCountText.text = string.Empty;
        }

        if (slotType == SlotType.Quick)
        {
            this.item = itemInstance;
            itemIcon.sprite = ItemResourceCache.GetIcon(itemInstance.data.iconPath);
            itemIcon.enabled = true;
            // 퀵슬롯 사이즈/특수처리 필요시 추가 (예: rt.sizeDelta = 고정값)
            return;
        }

        if (isRelicSlot || GridX == itemInstance.AnchorX && GridY == itemInstance.AnchorY)
        {
            this.item = itemInstance;
            itemIcon.sprite = ItemResourceCache.GetIcon(itemInstance.data.iconPath);
            itemIcon.enabled = true;
            
            if (slotType == SlotType.Normal || slotType == SlotType.Chest)
            {
                if (isViewingChestInventory)
                {
                    rt.sizeDelta = new Vector2(itemInstance.SizeX * 70 + (itemInstance.SizeX - 1) * 15,
                                           itemInstance.SizeY * 70 + (itemInstance.SizeY - 1) * 15);
                }
                else
                {
                    rt.sizeDelta = new Vector2(itemInstance.SizeX * 95 + (itemInstance.SizeX - 1) * 15,
                                               itemInstance.SizeY * 95 + (itemInstance.SizeY - 1) * 15);
                }
            }
        }
        else
        {
            ClearSlot(); // 루트 슬롯이 아니면 안 보여줌
            this.item = itemInstance;
            rt.sizeDelta = Vector2.zero;
        }
    }

    public void ClearSlot()
    {
        bool isViewingChestInventory = IsInChestUI();

        item = null;
        itemIcon.sprite = null;
        itemIcon.enabled = false;
        itemCountText.text = string.Empty;

        //rt.sizeDelta = new Vector2(95, 95);
        // 슬롯 타입에 따라 사이즈 다르게!
        if (slotType == SlotType.Normal || slotType == SlotType.Chest)
        {
            rt.sizeDelta = isViewingChestInventory ? new Vector2(70, 70) : new Vector2(95, 95);
        }
        else if (slotType == SlotType.Quick)
        {
            rt.sizeDelta = isViewingChestInventory ? new Vector2(75, 75) : new Vector2(100, 100); // 혹은 QuickSlot 프리팹 원본값과 일치!
        }
        else if (slotType == SlotType.Relic)
        {
            rt.sizeDelta = isViewingChestInventory ? new Vector2(70, 70) : new Vector2(95, 95);
        } 
    }

    private bool IsInChestUI()
    {
        if (slotType == SlotType.Chest) return false;  // 창고 슬롯의 경우 무시

        return GetComponentInParent<ChestUI>() != null;
    }
    #endregion

    #region 사용 불가 슬롯 색 변경
    public void UpdateBlockedSlots()
    {
        if (blockImage.sprite != null)
        {
            Color color = blockImage.color;
            color.a = blockImage.sprite != null ? 1f : 0f;
            blockImage.color = color;
            return;
        }

        var cell = InventoryManager.Instance.InventoryUI.Grid.GetCell(GridX, GridY);
        slotImage.color = (cell != null && cell.isBlocked) ? blockedTintColor : baseColor;
    }
    #endregion

    #region 인벤토리 내 아이템 표시
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (InputUI.Instance != null && InputUI.Instance.IsActive) return;
        if (DragSlotUI.Instance.draggedItem != null) return;
        highlight?.HighlightOn();

        if (item == null) return;

        isCursorEnter = true;

        if (parentInventoryUI == null)
        {
            Debug.LogWarning("[SlotUI] parentInventoryUI is null on OnPointerEnter!");
            return;
        }
        parentInventoryUI.SetItemInfo(item);

        //InventoryManager.Instance.InventoryUI.SetItemInfo(item);

        // 소모 아이템일 경우 퀵슬롯 등록
        StartCoroutine(WaitForQuickSlotKey());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isDragging) return;

        isCursorEnter = false;
        highlight?.HighlightOff();
        if (InputUI.Instance != null && InputUI.Instance.IsActive) return;

        if (parentInventoryUI != null)
            parentInventoryUI.ClearItemInfo();
        // 해당 슬롯의 아이템 정보 해제
        // InventoryManager.Instance.InventoryUI.ClearItemInfo();
    }
    #endregion

    #region 퀵슬롯 등록
    private IEnumerator WaitForQuickSlotKey()
    {
        // 현재 플레이어 컨트롤러 참조
        var player = InventoryManager.Instance?.InventoryUI?.Player;
        if (player == null)
            yield break;

        // 플레이어 인풋 매니저에서 최신 인풋 데이터 참조
        InputManager inputManager = player.InputManager;

        while (isCursorEnter)
        {
            if (InputUI.Instance != null && InputUI.Instance.IsActive)
            {
                yield return null;
                continue;
            }

            ItemInstance targetItem = item;

            if (targetItem == null && ParentInventoryUI != null)
            {
                var grid = ParentInventoryUI.Grid;
                targetItem = grid?.GetItemAt(GridX, GridY);

                if (targetItem == null)
                {
                    foreach (var inst in grid.GetAllItems())
                    {
                        for (int dx = 0; dx < inst.SizeX; dx++)
                            for (int dy = 0; dy < inst.SizeY; dy++)
                            {
                                if (inst.AnchorX + dx == GridX && inst.AnchorY + dy == GridY)
                                {
                                    targetItem = inst;
                                    break;
                                }
                            }
                        if (targetItem != null) break;
                    }
                }
            }

            if (targetItem != null && targetItem.data.type == "Consumable")
            {
                inputManager.UpdateInputData(); // 인풋 갱신
                var input = inputManager.GetInput();

                if (input.isUseItem1Down)
                {
                    QuickSlotManager.Instance.SetQuickSlot(0, targetItem);
                    break;
                }
                else if (input.isUseItem2Down)
                {
                    QuickSlotManager.Instance.SetQuickSlot(1, targetItem);
                    break;
                }
            }
            yield return null;
        }
    }
    #endregion

    #region 드래그 앤 드롭
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (InputUI.Instance != null && InputUI.Instance.IsActive) return;

        isDragging = true;

        if (item != null)
        {
            DragSlotUI.Instance.draggedItem = item;
            DragSlotUI.Instance.draggedItemAnchorX = item.AnchorX;
            DragSlotUI.Instance.draggedItemAnchorY = item.AnchorY;

            DragSlotUI.Instance.rt.sizeDelta = new Vector2((item.SizeX * 95 + (item.SizeX - 1) * 25) * 1.1f,
                                                           (item.SizeY * 95 + (item.SizeY - 1) * 25) * 1.1f);

            DragSlotUI.Instance.DragSetImage(itemIcon);

            DragSlotUI.Instance.transform.position = eventData.position;

            if (isRelicSlot)
            {
                DragSlotUI.Instance.draggedRelicSlotIndex = GridX;
            }

            DragSlotUI.Instance.sourceSlot = this;
        }
        parentInventoryUI.DetectSlots(true);
        // InventoryManager.Instance.InventoryUI.DetectSlots(true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (InputUI.Instance != null && InputUI.Instance.IsActive) return;

        if (item != null)
            DragSlotUI.Instance.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        DragSlotUI.Instance.SetColor(0);
        DragSlotUI.Instance.draggedItem = null;
        DragSlotUI.Instance.draggedItemAnchorX = -1;
        DragSlotUI.Instance.draggedItemAnchorY = -1;
        DragSlotUI.Instance.draggedRelicSlotIndex = -1;
        DragSlotUI.Instance.rt.sizeDelta = Vector2.zero;

        isDragging = false;
        parentInventoryUI.DetectSlots(false);
        //InventoryManager.Instance.InventoryUI.DetectSlots(false);
        highlight?.HighlightOff();

        //if (IsDroppedOutsideGrid(eventData))
        //{
        //    parentInventoryUI.ThrowItem(item, item.stackCount);
        //}

        //DragSlotUI.Instance.sourceSlot = null;



    }

    public void OnDrop(PointerEventData eventData)
    {
        ItemInstance draggedItem = DragSlotUI.Instance.draggedItem;
        int fromX = DragSlotUI.Instance.draggedItemAnchorX;
        int fromY = DragSlotUI.Instance.draggedItemAnchorY;
        int fromRelicIndex = DragSlotUI.Instance.draggedRelicSlotIndex;

        // 드래그 시작 슬롯
        SlotUI sourceSlot = DragSlotUI.Instance.sourceSlot;

        var sourceUI = sourceSlot?.ParentInventoryUI;
        var targetUI = this.ParentInventoryUI;

        var sourceGrid = sourceSlot?.ParentGrid;
        var targetGrid = this.ParentGrid;

        if (draggedItem == null) return;

        // Quick Slot
        if (isQuickSlot && Enum.TryParse<ItemType>(draggedItem.data.type, out var quickType) && quickType == ItemType.Consumable)
        {
            if (sourceSlot != null && sourceSlot.slotType == SlotType.Chest)
            {
                return;
            }

            int quickSlotIdx = this.QuickSlotIndex;
            var player = InventoryManager.Instance.InventoryUI.Player;
            if (player != null)
                player.QuickSlots[quickSlotIdx] = draggedItem;
            QuickSlotManager.Instance.SetQuickSlot(quickSlotIdx, draggedItem);
            return;
        }

        // Relic Slot
        if (isRelicSlot && Enum.TryParse<ItemType>(draggedItem.data.type, out var dropType) && dropType == ItemType.Relic)
        {
            sourceGrid?.RemoveItem(draggedItem);

            var player = InventoryManager.Instance.InventoryUI.Player;
            if (player == null)
            {
                Debug.LogError("[SlotUI] 유물 슬롯 드롭: Player가 null");
                return;
            }
            player.EquipItem(draggedItem, GridX);

            return;
        }

        if (!isRelicSlot && fromRelicIndex >= 0 && Enum.TryParse<ItemType>(draggedItem.data.type, out var fromType) && fromType == ItemType.Relic)
        {
            var player = InventoryManager.Instance.InventoryUI.Player;

            // 1. 유물 해제
            player.UnEquipItem(draggedItem);

            // 2. 대상 인벤토리 그리드에 추가 (플레이어/상자 모두)
            if (targetGrid != null)
            {
                if (!targetGrid.AddItem(draggedItem))
                {
                    // 실패 시 복구: 다시 유물 장착
                    player.EquipItem(draggedItem, fromRelicIndex);
                }
            }

            sourceUI?.UpdateUI();
            targetUI?.UpdateUI();
            return;
        }

        // [1] 플레이어 ↔ 창고 간 아이템 이동
        if (sourceGrid != null && targetGrid != null && sourceGrid != targetGrid)
        {
            var newInstance = new ItemInstance(draggedItem.data)
            {
                stackCount = draggedItem.stackCount,
            };

            sourceGrid.RemoveItem(draggedItem);

            if (QuickSlotManager.Instance.HasItem(draggedItem))
            {
                draggedItem.stackCount = 0;
            }

            if (!targetGrid.AddItem(newInstance))
            {
                // 실패 시 복구
                sourceGrid.AddItem(newInstance);
            }

            sourceUI?.UpdateUI();
            targetUI?.UpdateUI();
            sourceUI?.QuickSlotPanelUI?.UpdateUI();
            return;
        }


        if (Enum.TryParse<ItemType>(draggedItem.data.type, out var type))
        {
            if (isRelicSlot && type == ItemType.Relic)
            {
                return;
            }
            else if (!isRelicSlot && fromRelicIndex >= 0 && type == ItemType.Relic)
            {
                return;
            }
            else
            {
                HandleDropWithinInventory(draggedItem, fromX, fromY);
                return;
            }
        }

        if (IsDroppedOutsideGrid(eventData))
        {
            targetUI?.ThrowItem(draggedItem, draggedItem.stackCount);

            return;
        }
    }

    private bool IsDroppedOutsideGrid(PointerEventData eventData)
    {
        // parentInventoryUI의 slotParent 등 슬롯 영역 RectTransform 참조
        RectTransform gridRect = parentInventoryUI.GetComponentInChildren<GridLayoutGroup>().GetComponent<RectTransform>();
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            gridRect, eventData.position, eventData.pressEventCamera, out localPoint);
        return !gridRect.rect.Contains(localPoint);
    }

    #endregion

    #region 아이템 드롭 관련 함수

    #region 인벤토리 -> 유물 슬롯 드롭 / 유물 슬롯 내 드롭
    private void HandleDropToRelicSlot(ItemInstance draggedItem, int fromX, int fromY)
    {
        int toIndex = this.GridX; // 유물 슬롯 인덱스로 활용
        var player = InventoryManager.Instance.InventoryUI.Player;
        var relics = player.GetEquippedRelics();
        var existingItem = relics.ElementAtOrDefault(toIndex);

        // draggedItem이 이미 유물 슬롯에 장착된 상태이고, 다른 유물 슬롯으로 옮기려는 경우
        if (player.IsEquipped(draggedItem))
        {
            int fromIndex = DragSlotUI.Instance.draggedRelicSlotIndex;

            // 서로 다른 슬롯일 때만 스왑
            if (fromIndex != toIndex && fromIndex >= 0)
            {
                player.SwapRelics(fromIndex, toIndex);
            }

            return;
        }

        InventoryManager.Instance.InventoryUI.Grid.RemoveItem(draggedItem);

        // 기존 유물이 존재할 경우
        if (existingItem != null && existingItem != draggedItem)
        {
            if (!InventoryManager.Instance.InventoryUI.Grid.CanPlaceItem(existingItem, fromX, fromY))
            {
                InventoryManager.Instance.InventoryUI.Grid.PlaceItem(draggedItem, fromX, fromY);
                return;
            }

            player.UnEquipItem(existingItem);
            player.EquipItem(draggedItem, toIndex);
            InventoryManager.Instance.InventoryUI.Grid.PlaceItem(existingItem, fromX, fromY);
        }
        else if (!player.IsEquipped(draggedItem))
        {
            player.EquipItem(draggedItem, toIndex);
        }
    }
    #endregion

    #region 유물 슬롯 -> 인벤토리 드롭
    private void HandleDropFromRelicSlotToInventory(ItemInstance draggedItem, int fromRelicIndex)
    {
        var player = InventoryManager.Instance.InventoryUI.Player;
        ItemInstance targetItem = this.item;
        int toX = this.GridX;
        int toY = this.GridY;

        if (targetItem != null && targetItem != draggedItem)
        {
            // 해당 슬롯에 다른 유물 아이템이 이미 존재할 경우
            if (Enum.TryParse<ItemType>(targetItem.data.type, out var type) && type == ItemType.Relic)
            {
                int fromX = this.item.AnchorX;
                int fromY = this.item.AnchorY;

                InventoryManager.Instance.InventoryUI.Grid.RemoveItem(item);

                if (!InventoryManager.Instance.InventoryUI.Grid.CanPlaceItem(draggedItem, fromX, fromY))
                {
                    InventoryManager.Instance.InventoryUI.Grid.PlaceItem(targetItem, fromX, fromY);
                    return;
                }

                player.UnEquipItem(draggedItem);
                player.EquipItem(targetItem, fromRelicIndex);
                InventoryManager.Instance.InventoryUI.Grid.PlaceItem(draggedItem, fromX, fromY);
                return;
            }
        }

        // 일반적인 드롭 (타겟이 비어있거나 같은 경우)
        if (!InventoryManager.Instance.InventoryUI.Grid.CanPlaceItem(draggedItem, toX, toY))
            return;

        player.UnEquipItem(draggedItem);
        InventoryManager.Instance.InventoryUI.Grid.PlaceItem(draggedItem, toX, toY);
    }
    #endregion

    #region 인벤토리 내 드롭
    private void HandleDropWithinInventory(ItemInstance draggedItem, int fromX, int fromY)
    {
        var player = InventoryManager.Instance.InventoryUI.Player;
        ItemInstance targetItem = this.Item;
        int toX = this.GridX;
        int toY = this.GridY;

        player.UnEquipItem(draggedItem);

        var grid = this.ParentGrid; // 이 슬롯이 속한 그리드 참조

        if (targetItem != null && draggedItem != targetItem)
        {
            toX = targetItem.AnchorX;
            toY = targetItem.AnchorY;
            grid.TrySwapItems(draggedItem, fromX, fromY, targetItem, toX, toY);
        }
        else
        {
            grid.TryMoveItem(draggedItem, fromX, fromY, toX, toY);
        }
    }
    #endregion

    #endregion

    public void SetParentInventoryUI(InventoryUI ui)
    {
        parentInventoryUI = ui;
    }

    public void SetViewMode(InventoryGridViewMode mode)
    {
        viewMode = mode;
    }

    public InventoryGrid ParentGrid
    {
        get
        {
            return ParentInventoryUI != null ? ParentInventoryUI.Grid : null;
        }
    }
}

public static class ItemResourceCache
{
    private static Dictionary<string, Sprite> iconCache = new Dictionary<string, Sprite>();

    public static Sprite GetIcon(string path)
    {
        if (string.IsNullOrEmpty(path)) return null;
        if (iconCache.TryGetValue(path, out var sprite))
            return sprite;

        sprite = Resources.Load<Sprite>(path);
        iconCache[path] = sprite;
        return sprite;
    }
}
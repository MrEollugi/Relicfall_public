using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridCell
{
    public bool isBlocked { get; set; }
    public ItemInstance itemRef { get; set; }

    public int X { get; private set; }
    public int Y { get; private set; }

    public GridCell(int x, int y)
    {
        isBlocked = false;
        itemRef = null;
        X = x;
        Y = y;
    }
}

public class InventoryGrid
{
    public event Action OnInventoryChanged;

    private int width;
    private int height;
    private GridCell[,] grid;
    private bool isInventory;
    public bool IsViewingChestInventory = false;

    public int Width => width;
    public int Height => height;

    public InventoryGrid(int width, int height, bool isInventory = true)
    {
        this.width = width;
        this.height = height;

        grid = new GridCell[width, height];
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                grid[x, y] = new GridCell(x, y);

        this.isInventory = isInventory;
    }

    public GridCell GetCell(int x, int y)
    {
        if (x < 0 || x >= width || y < 0 || y >= height)
            return null;

        return grid[x, y];
    }

    public ItemInstance GetItemAt(int x, int y)
    {
        return GetCell(x, y)?.itemRef;
    }

    #region Serialization

    public List<ItemInstance> GetAllItems()
    {
        var items = new List<ItemInstance>();
        var seen = new HashSet<ItemInstance>();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var cell = grid[x, y];
                if (cell.itemRef != null && !seen.Contains(cell.itemRef))
                {
                    items.Add(cell.itemRef);
                    seen.Add(cell.itemRef);
                }
            }
        }
        // Debug.Log($"[InventoryGrid] GetAllItems() -> {items.Count} items");
        return items;
    }

    public void LoadFromList(List<ItemInstanceSaveData> items)
    {
        // 기존 모든 아이템 해제
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                grid[x, y].itemRef = null;

        if (items == null) 
        {
            Debug.Log("[InventoryGrid] LoadFromList: items list is null");
            return; 
        }

        foreach (var save in items)
        {
            // AnchorX/Y 는 이전에 저장된 위치
            var itemData = ItemDatabase.Instance.GetItemById(save.itemId);
            if (itemData == null)
            {
                Debug.LogWarning($"[InventoryGrid] ItemData for id {save.itemId} not found!");
                continue;
            }
            var inst = ItemInstance.FromSaveData(save, itemData);
            //Debug.Log($"[InventoryGrid]   placing '{inst.data.id}' at ({inst.AnchorX},{inst.AnchorY})");
            PlaceItem(inst, inst.AnchorX, inst.AnchorY);
        }
        OnInventoryChanged?.Invoke();
        //Debug.Log($"[InventoryGrid] Loaded into grid: {GetAllItems().Count} items");
    }

    #endregion

    #region 아이템 배치 관련
    public bool CanPlaceItem(ItemInstance item, int startX, int startY)
    {
        for (int x = 0; x < item.SizeX; x++)
        {
            for (int y = 0; y < item.SizeY; y++)
            {
                int targetX = startX + x;
                int targetY = startY + y;

                if (targetX >= width || targetY >= height)
                    return false;

                //if (!InventoryManager.Instance.InventoryUI.HasSlot(targetX, targetY))
                //    return false;

                GridCell cell = GetCell(targetX, targetY);
                if (cell == null || cell.isBlocked || cell.itemRef != null)
                    return false;
            }
        }

        return true;
    }

    public bool PlaceItem(ItemInstance item, int startX, int startY)
    {
        if (!CanPlaceItem(item, startX, startY))
            return false;

        item.AnchorX = startX;
        item.AnchorY = startY;

        for (int x = 0; x < item.SizeX; x++)
        {
            for (int y = 0; y < item.SizeY; y++)
            {
                int gx = startX + x;
                int gy = startY + y;
                GridCell cell = GetCell(gx, gy);
                cell.itemRef = item;
                // Debug.Log($"[PlaceItem] {item.data.id} 할당 → ({gx},{gy})");
            }
        }

        OnInventoryChanged?.Invoke();
        return true;
    }
    #endregion

    #region 획득 시 아이템 추가
    public bool AddItem(ItemInstance itemData)
    {
        if (itemData == null) return false;

        if (itemData.IsStackable)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    ItemInstance existing = GetItemAt(x, y);

                    if (existing != null && existing.Id == itemData.data.id && existing.stackCount < existing.MaxStackCount)
                    {
                        int canAdd = existing.MaxStackCount - existing.stackCount;
                        int toAdd = Mathf.Min(itemData.stackCount, canAdd);

                        existing.stackCount += toAdd;
                        itemData.stackCount -= toAdd;;

                        if (itemData.stackCount <= 0)
                        {
                            OnInventoryChanged?.Invoke();

                            if (isInventory && GameManager.Instance != null && GameManager.Instance.CurrentPlayer != null)
                                GameManager.Instance.CurrentPlayer.ApplyWeight(itemData);

                            return true;
                        }
                    }
                }
            }
        }

        for (int y = 0; y <= height - itemData.SizeY; y++)
        {
            for (int x = 0; x <= width - itemData.SizeX; x++)
            {
                if (CanPlaceItem(itemData, x, y))
                {
                    PlaceItem(itemData, x, y);
                    OnInventoryChanged?.Invoke();

                    if (isInventory && GameManager.Instance != null && GameManager.Instance.CurrentPlayer != null)
                        GameManager.Instance.CurrentPlayer.ApplyWeight(itemData);

                    return true;
                }
            }
        }

        return false;
    }
    #endregion

    #region 아이템 제거
    private void RemoveItemArea(ItemInstance item, int startX, int startY)
    {
        if (item == null) return;

        for (int x = 0; x < item.SizeX; x++)
        {
            for (int y = 0; y < item.SizeY; y++)
            {
                GridCell cell = GetCell(startX + x, startY + y);
                if (cell != null && cell.itemRef == item)
                {
                    cell.itemRef = null;
                }
            }
        }

        if (isInventory && GameManager.Instance != null && GameManager.Instance.CurrentPlayer != null)
            GameManager.Instance.CurrentPlayer.RemoveWeight(item);
    }

    public void RemoveItem(ItemInstance itemToDrop)
    {
        int currentX = itemToDrop.AnchorX;
        int currentY = itemToDrop.AnchorY;

        if (GetItemAt(currentX, currentY) != itemToDrop) return;

        RemoveItemArea(itemToDrop, currentX, currentY);

        OnInventoryChanged?.Invoke();
    }
    #endregion

    #region 아이템 사용
    public void UseItem(ItemInstance item)
    {
        if (item == null) return;

        if (item.IsStackable && item.stackCount > 1)
        {
            item.stackCount--;
            return;
        }

        RemoveItemArea(item, item.AnchorX, item.AnchorY);
        OnInventoryChanged?.Invoke();
    }
    #endregion

    #region 아이템 이동
    public bool TryMoveItem(ItemInstance itemToMove, int currentX, int currentY, int targetX, int targetY)
    {
        RemoveItemArea(itemToMove, currentX, currentY);

        if (CanPlaceItem(itemToMove, targetX, targetY))
        {
            PlaceItem(itemToMove, targetX, targetY);
            OnInventoryChanged?.Invoke();
            return true;
        }
        else
        {
            PlaceItem(itemToMove, currentX, currentY);
            OnInventoryChanged?.Invoke();
            return false;
        }
    }

    public bool TrySwapItems(ItemInstance currentItem, int currentX, int currentY, ItemInstance targetItem, int targetX, int targetY)
    {
        RemoveItemArea(currentItem, currentX, currentY);

        bool item2Placed = PlaceItem(targetItem, currentX, currentY);

        if (item2Placed)
        {
            RemoveItemArea(targetItem, targetX, targetY);

            if (CanPlaceItem(currentItem, targetX, targetY))
            {
                PlaceItem(currentItem, targetX, targetY);
                OnInventoryChanged?.Invoke();
                Debug.Log("교체 성공");
                return true;
            }
            else
            {
                RemoveItemArea(targetItem, currentX, currentY);
                PlaceItem(currentItem, currentX, currentY);
                PlaceItem(targetItem, targetX, targetY);
                OnInventoryChanged?.Invoke();
                Debug.Log("교체 실패");
                return false;
            }
        }
        else
        {
            PlaceItem(currentItem, currentX, currentY);
            OnInventoryChanged?.Invoke();
            Debug.Log("교체 실패");
            return false;
        }
    }
    #endregion

    #region 그리드 크기 재조정 (수정 필요)
    public void ResizeGrid(int newWidth, int newHeight)
    {
        GridCell[,] newGrid = new GridCell[newWidth, newHeight];

        for (int x = 0; x < newWidth; x++)
        {
            for (int y = 0; y < newHeight; y++)
            {
                newGrid[x, y] = new GridCell(x, y);
            }
        }

        HashSet<ItemInstance> copiedItems = new HashSet<ItemInstance>();

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                GridCell oldCell = grid[x, y];
                ItemInstance item = oldCell.itemRef;

                if (item != null && !copiedItems.Contains(item))
                {
                    if (item.AnchorX < newWidth && item.AnchorY < newHeight)
                    {
                        for (int i = 0; i < item.SizeX; i++)
                        {
                            for (int j = 0; j < item.SizeY; j++)
                            {
                                int targetX = item.AnchorX + i;
                                int targetY = item.AnchorY + j;

                                if (targetX < newWidth && targetY < newHeight)
                                {
                                    newGrid[targetX, targetY].itemRef = item;
                                }
                            }
                        }
                        copiedItems.Add(item);
                    }
                }
            }
        }

        grid = newGrid;
        width = newWidth;
        height = newHeight;

        OnInventoryChanged?.Invoke();
    }
    #endregion

    #region 사용 불가 슬롯
    public ItemInstance BlockRandomCells()
    {
        List<GridCell> cells = new List<GridCell>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (InventoryManager.Instance.InventoryUI.HasSlot(x, y))
                {
                    GridCell cell = GetCell(x, y);
                    if (cell != null && !cell.isBlocked) // 이미 막혀 있는 셀은 제외
                        cells.Add(cell);
                }
            }
        }

        if (cells.Count == 0)
            return null;

        int index = UnityEngine.Random.Range(0, cells.Count);
        GridCell selected = cells[index];
        selected.isBlocked = true;
        cells.RemoveAt(index);

        OnInventoryChanged?.Invoke(); // UI 갱신 트리거

        if (selected.itemRef != null)
            return selected.itemRef;

        return null;
    }

    public void UnblockCells()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GridCell cell = GetCell(x, y);
                if (cell != null)
                    cell.isBlocked = false;
            }
        }

        OnInventoryChanged?.Invoke(); // UI 갱신
    }
    #endregion

    #region Inventory Reset
    public void Clear()
    {
        // 모든 셀의 아이템 참조를 null로
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                grid[x, y].itemRef = null;

        OnInventoryChanged?.Invoke(); // UI 갱신 등
    }
    #endregion
}

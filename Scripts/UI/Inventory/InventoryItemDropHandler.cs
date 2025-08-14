using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryItemDropHandler : MonoBehaviour, IDropHandler
{
    private ItemInstance draggedItem;

    public void OnDrop(PointerEventData eventData)
    {
        draggedItem = DragSlotUI.Instance.draggedItem;
        var sourceSlot = DragSlotUI.Instance.sourceSlot;
        if (draggedItem == null) return;

        if (draggedItem.stackCount == 1)
        {
            InventoryManager.Instance.InventoryUI.ThrowItem(draggedItem, 1);
            return;
        }
        
        InputUI.Instance.Show(OnConfirmDropCount, draggedItem.stackCount);
    }

    private void OnConfirmDropCount(int dropCount)
    {
        int dropItemCount = Mathf.Min(dropCount, draggedItem.stackCount);

        InventoryManager.Instance.InventoryUI.ThrowItem(draggedItem, dropItemCount);
        InventoryManager.Instance.InventoryUI.ClearItemInfo();

        DragSlotUI.Instance.draggedItem = null;
        DragSlotUI.Instance.draggedItemAnchorX = -1;
        DragSlotUI.Instance.draggedItemAnchorY = -1;
        DragSlotUI.Instance.rt.sizeDelta = Vector2.zero;

        draggedItem = null; // 사용 후 초기화
    }
}
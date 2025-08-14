using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour, IImmediateInteractable
{
    [SerializeField] private ItemData item;

    public void Interact(PlayerController player)
    {
        InventoryGrid grid = player.Inventory;  // InventoryGrid
        ItemInstance inst = new ItemInstance(item) { stackCount = 1 };
        player.AddItemToInventory(inst);
        Debug.Log("현재 인벤토리 아이템 개수: " + grid.GetAllItems().Count);
        // InventoryUI는 InventoryManager를 통해 가져오기
        InventoryUI ui = InventoryManager.Instance.InventoryUI;
        if (ui != null && ui.IsOpen)
        {
            ui.UpdateUI();
            Debug.Log("[ItemPickup] InventoryUI.UpdateUI() via InventoryManager");
        }

        Destroy(gameObject);
    }
}

using UnityEngine;

public class InventoryTest : MonoBehaviour
{
    [SerializeField] private InventoryUI inventoryUI;
    [SerializeField] private ItemData item;
    [SerializeField] private int blockSlotsCount = 1;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            inventoryUI.Toggle();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ItemInstance itemToAdd = new ItemInstance(item);

            inventoryUI.Grid.AddItem(itemToAdd);
            Debug.Log("Item added.");
            inventoryUI.UpdateUI();
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            inventoryUI.AddSlots();
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            inventoryUI.SetBlockSlots(blockSlotsCount);
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            inventoryUI.UnblockSlots();
        }
    }
}

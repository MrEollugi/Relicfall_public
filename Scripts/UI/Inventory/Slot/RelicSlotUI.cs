using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RelicSlotUI : MonoBehaviour
{
    [SerializeField] private Transform slotParent;
    [SerializeField] private GameObject slotPrefab;
    private List<SlotUI> relicSlots = new List<SlotUI>();

    private InventoryUI inventoryUI;

    private PopUpUI popUp;

    private void Awake()
    {
        popUp = GetComponent<PopUpUI>();
    }

   public void Init(InventoryUI inventoryUI)
    {
        this.inventoryUI = inventoryUI;

        if (relicSlots.Count > 0)
        {
            foreach (var slot in relicSlots)
                slot.SetParentInventoryUI(inventoryUI);

            UpdateUI();
            return;
        }

        for (int i = 0; i < 2; i++)
        {
            var newSlot = Instantiate(slotPrefab, slotParent);
            var slot = newSlot.GetComponentInChildren<SlotUI>();

            slot.SetRelicSlot(true);
            slot.SetParentInventoryUI(inventoryUI);
            slot.Init(i, 0);
            slot.ClearSlot();

            relicSlots.Add(slot);
        }

        UpdateUI();
    }

    public void UpdateUI()
    {
        var player = GameManager.Instance.CurrentPlayer;
        var equippedRelics = player.GetEquippedRelics();

        for (int i = 0; i < relicSlots.Count; i++)
        {
            var relic = equippedRelics.ElementAtOrDefault(i);
            relicSlots[i].SetItem(relic);
        }
    }

    public void PopUp()
    {
        popUp.Show();
    }
    
}

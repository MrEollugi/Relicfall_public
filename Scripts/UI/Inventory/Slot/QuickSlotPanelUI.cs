using System.Collections.Generic;
using UnityEngine;

public class QuickSlotPanelUI : MonoBehaviour
{
    public List<SlotUI> QuickSlots; // 인스펙터에서 2개 drag&drop (slotType=Quick)

    [SerializeField] private PopUpUI popUp;

    private void Awake()
    {
        popUp = GetComponent<PopUpUI>();

        for (int i = 0; i < QuickSlots.Count; i++)
        {
            QuickSlots[i].Init(i, 0);
            QuickSlots[i].SetViewMode(InventoryGridViewMode.Player);
            QuickSlots[i].SetQuickSlot(true);
            QuickSlots[i].SetQuickSlotIndex(i);
        }
    }

    private void Start()
    {
        var inventoryUI = InventoryManager.Instance.InventoryUI;
        foreach (var slot in QuickSlots)
        {
            slot.SetParentInventoryUI(inventoryUI);
        }
    }

    private void OnEnable()
    {
        QuickSlotManager.Instance.OnQuickSlotChanged += SyncQuickSlots;
        UpdateUI();
    }
    private void OnDisable()
    {
        if (QuickSlotManager.Instance != null)
            QuickSlotManager.Instance.OnQuickSlotChanged -= SyncQuickSlots;
    }

    private bool IsInChestUI()
    {
        return GetComponentInParent<ChestUI>() != null;
    }

    //public void SetQuickSlot(int idx, ItemInstance item)
    //{

    //    if (idx < 0 || idx >= QuickSlots.Count) return;

    //    for (int i = 0; i < QuickSlotManager.Instance.QuickSlots.Length; i++)
    //    {
    //        var q = QuickSlotManager.Instance.QuickSlots[i];
    //        if ((q != null && q == item) || (q != null && item != null && q.data.id == item.data.id))
    //        {
    //            QuickSlotManager.Instance.QuickSlots[i] = null;
    //        }
    //    }

    //    QuickSlotManager.Instance.QuickSlots[idx] = item;
    //    QuickSlots[idx].SetItem(item);

    //    popUp?.Show();
    //}

    public void SyncQuickSlots()
    {
        bool IsViewingChestInventory = IsInChestUI();

        var quickSlots = QuickSlotManager.Instance.QuickSlots;

        for (int i = 0; i < QuickSlots.Count; i++)
        {
            var item = i < quickSlots.Length ? quickSlots[i] : null;
            QuickSlots[i].SetItem(item);  
        }
    }

    public void PopUp()
    {
        popUp?.Show();
    }

    public void UpdateUI()
    {
        SyncQuickSlots();
    }
}

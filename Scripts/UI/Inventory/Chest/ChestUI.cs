using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ChestUI : MonoBehaviour
{
    [Header("패널/인벤토리")]
    public InventoryUI playerInventoryUI;
    public InventoryUI chestInventoryUI;

    public event Action OnClosed;

    public void Init(PlayerController player, InventoryGrid chestGrid)
    {

        if (playerInventoryUI != null)
            playerInventoryUI.gameObject.SetActive(true);
        if (chestInventoryUI != null)
            chestInventoryUI.gameObject.SetActive(true);

        // 1. 플레이어 인벤토리/창고 인벤토리 초기화
        chestInventoryUI.Init(chestGrid.Width, chestGrid.Height, chestGrid, InventoryGridViewMode.Chest);
        playerInventoryUI.Init(player.Inventory.Width, player.Inventory.Height, player.Inventory, InventoryGridViewMode.Chest, true);

        playerInventoryUI?.RelicSlotUI?.PopUp();
        playerInventoryUI?.QuickSlotPanelUI?.PopUp();
        // (필요하면) 드래그 앤 드랍, 아이템 이동 이벤트 연결 등 추가
    }

    private void Awake()
    {

    }

    public void Close()
    {
        if (playerInventoryUI != null)
            playerInventoryUI.gameObject.SetActive(false);
        if (chestInventoryUI != null)
            chestInventoryUI.gameObject.SetActive(false);
        
        OnClosed?.Invoke();
        //Destroy(gameObject);

        gameObject.SetActive(false);
    }
}

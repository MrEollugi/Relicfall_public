using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoloSlotPanel : MonoBehaviour
{
    public SaveSlotPanel[] slotPanels; // Slot1Panel, Slot2Panel, Slot3Panel

    private void Awake()
    {
        for (int i = 0; i < slotPanels.Length; i++)
        {
            string slotId = i.ToString();
            var player = new SaveManager().LoadPlayer(slotId, GameMode.Single);
            var world = new SaveManager().LoadWorld(slotId, GameMode.Single);
            slotPanels[i].SetSlotData(slotId, player, world);
        }
    }

    public void RefreshSlots()
    {
        for (int i = 0; i < slotPanels.Length; i++)
        {
            string slotId = i.ToString();
            var player = new SaveManager().LoadPlayer(slotId, GameMode.Single);
            var world = new SaveManager().LoadWorld(slotId, GameMode.Single);
            slotPanels[i].SetSlotData(slotId, player, world);
        }
    }
}

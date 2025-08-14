using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuickSlotManager : MonoBehaviour
{
    private static QuickSlotManager instance;
    public static QuickSlotManager Instance
    {
        get
        {
            //if (instance == null)
            //{
            //    GameObject go = new GameObject("QuickSlotManager");
            //    instance = go.AddComponent<QuickSlotManager>();
            //    DontDestroyOnLoad(go);
            //}
            return instance;
        }
    }

    public ItemInstance[] QuickSlots = new ItemInstance[2];

    public event Action OnQuickSlotChanged;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetQuickSlot(int idx, ItemInstance item)
    {
        if (idx < 0 || idx >= QuickSlots.Length) return;

        for (int i = 0; i < QuickSlots.Length; i++)
        {
            var q = QuickSlots[i];
            if ((q != null && q == item) || (q != null && item != null && q.data.id == item.data.id))
            {
                QuickSlots[i] = null;
            }
        }

        QuickSlots[idx] = item;
        OnQuickSlotChanged?.Invoke();
    }

    public bool HasItem(ItemInstance item)
    {
        return QuickSlots.Any(slot => slot == item);
    }
}

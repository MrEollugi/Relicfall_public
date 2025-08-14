using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public enum StorePanelType
{
    Sell,
    Buy
}


public class StorePanel : MonoBehaviour
{
    [SerializeField] private Store store;
    [SerializeField] private StoreNPC storeNPC;

    public StorePanelType type;
    public TextMeshProUGUI purchaseCostText;
    public TextMeshProUGUI purchaseListText;
    public TextMeshProUGUI goldText;

    public List<ItemInstance> purchaseList = new List<ItemInstance>();//플레이어가 구매할 물건들의 리스트

    public List<string> sellList = new List<string>();//상점에서 판매하는 물건들.

    public Transform itemParent;//ui
    public GameObject sellItem;//ui
    public int purchaseCost;

    void Start()
    {
        store = FindObjectOfType<Store>();

        if (store == null)
            Debug.Log("store is Null");

        if (type == StorePanelType.Buy)
            SetItemList();
    }

    void Update()
    {
        if (type == StorePanelType.Sell)
            SetGoldText();
        else if (type == StorePanelType.Buy)
            RefreshText();
    }

    void SetGoldText()
    {
        goldText.text = store.CalReward().ToString();
    }

    void RefreshText()
    {
        purchaseCostText.text = purchaseCost.ToString();
        purchaseListText.text = ListToText();
    }

    string ListToText()
    {
        Dictionary<string, int> itemCounts = new Dictionary<string, int>();

        foreach (var obj in purchaseList)
        {
            ItemData item = obj.data;
            if (item == null) continue;

            if (itemCounts.ContainsKey(item.name))
                itemCounts[item.name]++;
            else
                itemCounts[item.name] = 1;
        }

        StringBuilder sb = new StringBuilder();
        foreach (var pair in itemCounts)
        {
            sb.AppendLine($"{pair.Key}({pair.Value})");
        }
        return sb.ToString();
    }

    public void SetItemList()
    {
        for (int i = 0; i < sellList.Count; i++)
        {
            var itemData = ItemDatabase.Instance.GetItemById(sellList[i]);
            var newSellItem = Instantiate(sellItem, itemParent);

            StoreItemSet itemSet = newSellItem.GetComponent<StoreItemSet>();
            itemSet.Set(itemData, this);
        }
    }

    public void AddItem(ItemInstance item)
    {
        purchaseList.Add(item);
        if (item.data != null)
            purchaseCost += item.data.value;
        else
            Debug.Log("아이템 없음");
    }

    public void RemoveItem(ItemInstance item)
    {
        int idx = purchaseList.FindIndex(x => x.data != null && item.data != null && x.data.id == item.data.id);


        if (idx >= 0)
        {
            purchaseCost -= purchaseList[idx].data.value;
            purchaseList.RemoveAt(idx);
        }
        else
        {
            return;
        }

    }

    public void RemoveAll()
    {
        purchaseList.Clear();
        purchaseCost = 0;
    }

    public void PurchaseItemInList()
    {
        bool b = GameManager.Instance.SpendGold(purchaseCost);

        if (b == false)
        {
            Debug.Log("돈없음");
            return;
        }
        else
        {
            foreach (ItemInstance item in purchaseList)
            {
                GameManager.Instance.CurrentPlayer.AddItemToInventory(item);
            }
            RemoveAll();
        }

    }

    private bool IsAlreadyInList(ItemInstance item)
    {
        return purchaseList.Any(x => x.data != null && item.data != null && x.data.id == item.data.id);
    }
}
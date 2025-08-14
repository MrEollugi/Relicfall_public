using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoreItemSet : MonoBehaviour//StorePanel에서 구매하는 아이템 만들때 텍스트 세팅하기 
{
    public Image sprite;
    public TextMeshProUGUI nameText;   
    public TextMeshProUGUI priceText;

    public Button plusButton;
    public Button minusButton;

    private ItemData itemData; // Set()에서 저장
    private StorePanel storePanel; // 어디에 아이템을 추가할지 참조

    public void Set(ItemData item, StorePanel panel)
    {
        if (nameText == null) Debug.LogError("nameText 미연결");
        if (priceText == null) Debug.LogError("priceText 미연결");
        if (item == null) Debug.LogError("item이 null");
        if (sprite == null) Debug.LogError("sprite 없음");
        itemData = item;        // 내부에 저장
        storePanel = panel;     // 패널도 저장

        plusButton.onClick.RemoveAllListeners();
        plusButton.onClick.AddListener(OnPlusClicked);

        minusButton.onClick.RemoveAllListeners();
        minusButton.onClick.AddListener(OnMinusClicked);

        //sprite = item.;
        nameText.text = item.name;
        priceText.text = item.value.ToString();
    }
    public ItemInstance GetItem()
    {
        if (storePanel == null || itemData == null)
        {
            Debug.LogWarning("storePanel 또는 itemData가 null임");
            return null;
        }

        ItemInstance itemInstance = new ItemInstance(itemData);
        Debug.Log($"{itemInstance}");
        return itemInstance;
    }

    private void OnPlusClicked()
    {
        if (storePanel == null || itemData == null)
        {
            Debug.LogWarning("storePanel 또는 itemData가 null임!");
            return;
        }
        storePanel.AddItem(new ItemInstance(itemData));
    }

    private void OnMinusClicked()
    {
        if (storePanel == null || itemData == null)
        {
            Debug.LogWarning("storePanel 또는 itemData가 null임!");
            return;
        }
        storePanel.RemoveItem(new ItemInstance(itemData));
    }

}

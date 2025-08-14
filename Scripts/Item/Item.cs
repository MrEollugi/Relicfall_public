
using UnityEngine;

public class Item : MonoBehaviour, IImmediateInteractable, IWorldTooltipDataProvider
{
    [SerializeField] private string itemId;
    public string ItemId { get => itemId; set => itemId = value; }

    private ItemInstance data;

    public void Init(ItemInstance instance)
    {
        data = instance;
        itemId = instance.data.id;
    }

    public ItemInstance GetData()
    {
        if (data != null) return data;

        // itemId를 통해 데이터베이스에서 가져옴
        var itemData = ItemDatabase.Instance.GetItemById(itemId);
        if (itemData == null)
            return null;

        return new ItemInstance(itemData);
    }

    public void Interact(PlayerController player)
    {
        ItemInstance itemToAdd = data ?? GetData();

        if (itemToAdd == null) return;

        if (player.AddItemToInventory(itemToAdd))
        {
            Destroy(gameObject);
        }
    }

    public TooltipType GetTooltipType()
    {
        return TooltipType.Item;
    }

    public void InitializeTooltip(GameObject tooltipInstance)
    {
        if (tooltipInstance.TryGetComponent<ItemTooltipUI>(out var tooltip))
        {
            tooltip.SetData(GetData());
        }
    }
}

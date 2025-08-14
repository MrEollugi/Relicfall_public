using UnityEngine;
using UnityEngine.UI;

public class DragSlotUI : MonoBehaviour
{
    public static DragSlotUI Instance;

    public Image itemImage;
    public float alpha = 0.7f;

    public RectTransform rt { get; private set; }

    public ItemInstance draggedItem { get; set; }
    public int draggedItemAnchorX = -1;
    public int draggedItemAnchorY = -1;
    public int draggedRelicSlotIndex = -1;

    public SlotUI sourceSlot { get; set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        rt = itemImage.GetComponent<RectTransform>();

        rt.anchorMin = new Vector2(0, 1);
        rt.anchorMax = new Vector2(0, 1);
        rt.pivot = new Vector2(0, 1);
    }

    public void DragSetImage(Image itemImage)
    {
        this.itemImage.sprite = itemImage.sprite;
        SetColor(alpha);
    }

    public void SetColor(float alpha)
    {
        Color color = itemImage.color;
        color.a = alpha;
        itemImage.color = color;
    }
}

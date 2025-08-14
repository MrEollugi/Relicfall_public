using UnityEngine;
using UnityEngine.EventSystems;

public class DetectSlot : MonoBehaviour, IDropHandler
{
    [SerializeField] private SlotUI slot;

    public void OnDrop(PointerEventData eventData)
    {
        slot.OnDrop(eventData);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TimeInteract 교체 가능성?
public class KeyDoor : MonoBehaviour, IImmediateInteractable
{
    public string keyID;
    private bool isOpen = false;

    public void Interact(PlayerController player)
    {
        if (isOpen) return;

        var items = player.Inventory.GetAllItems();

        foreach (var i in items)
        {
            if (i.Id == keyID)
            {
                player.Inventory.UseItem(i);
                ChangeToNormalDoor();
                return;
            }
        }

        // 열쇠가 없을 때 상호작용한 경우
        Debug.Log("열쇠 없음");
    }

    void ChangeToNormalDoor()
    {
        isOpen = true;
        // 일반 문을 현재 위치에 배치(교환)
        // 현재 문 삭제
        Destroy(gameObject);
    }
}

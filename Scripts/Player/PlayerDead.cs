using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDead : MonoBehaviour, IImmediateInteractable
{
    public string ownerPlayerId;

    // 죽은 플레이어의 정보 저장
    public void Init(string playerId)
    {
        ownerPlayerId = playerId;
    }

    // 아군이 이 오브젝트를 주웠을 때
    public void Interact(PlayerController picker)
    {
        // DeadItem(인벤토리용 아이템)에 playerId만 저장
        var deadItem = new ItemInstance("PlayerDead"); // 아이템 데이터 필요
        deadItem.ownerPlayerId = ownerPlayerId; // 확장 필요
        picker.AddItemToInventory(deadItem);
        Destroy(gameObject);
    }
}

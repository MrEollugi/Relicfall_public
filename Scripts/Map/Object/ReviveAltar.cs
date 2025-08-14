using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ReviveAltar : MonoBehaviour, ITimedInteractable
{
    private float interactionTime = 9f;
    public float InteractionTime => interactionTime;

    private PlayerController currentInteractor;

    public void OnInteractionComplete(PlayerController picker)
    {
        currentInteractor = null;
        var allItems = picker.Inventory.GetAllItems();
        var deadItem = allItems.FirstOrDefault(item => item.itemId == "PlayerDead");
        if (deadItem != null)
        {
            var deadPlayer = FindPlayerById(deadItem.ownerPlayerId);
            if (deadPlayer != null && !deadPlayer.gameObject.activeSelf)
            {
                deadPlayer.transform.position = this.transform.position;
                deadPlayer.gameObject.SetActive(true);
                picker.Inventory.RemoveItem(deadItem);
                Debug.Log("[ReviveAltar] 부활 성공!");
                // Revive effect, HP restore 등
            }
            else
            {
                Debug.Log("[ReviveAltar] 이미 살아있는 플레이어거나, 찾을 수 없음.");
            }
        }
        else
        {
            Debug.Log("[ReviveAltar] 부활 아이템(시체/머리)이 없음.");
        }
    }

    public void OnInteractionCanceled()
    {
        currentInteractor = null;
    }

    private PlayerController FindPlayerById(string playerId)
    {
        return GameObject.FindObjectsOfType<PlayerController>()
            .FirstOrDefault(pc => pc.playerId == playerId);
    }
}

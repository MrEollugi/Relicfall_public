using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NPCType
{
    Quest,
    Store,

}

public abstract class BaseNPC : MonoBehaviour, IImmediateInteractable, IWorldTooltipDataProvider
{
    [SerializeField] private int id;
    [SerializeField] private string name;
    [SerializeField] private NPCType type;

    public int Id => id;
    public string Name => name;
    public NPCType Type => type;

    // 상호작용 액션
    public event Action<PlayerController> OnInteracted;
    

    public virtual void Interact(PlayerController player)
    {
        OnInteracted?.Invoke(player);
    }

    public virtual void DialogFinished()
    {

    }
    public virtual void Act()
    {
        
    }

    public TooltipType GetTooltipType()
    {
        return TooltipType.NPC;
    }

    public void InitializeTooltip(GameObject tooltipInstance)
    {
        if (tooltipInstance.TryGetComponent<NPCTooltipUI>(out var tooltip))
        {
            tooltip.SetData(this);
        }
    }
}

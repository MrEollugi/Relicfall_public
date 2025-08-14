using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestNPC : BaseNPC
{
    [SerializeField] private int dialogDataIndex;

    public override void Interact(PlayerController player)
    {
        base.Interact(player);
        DialogSystem.Instance.StartDialog(dialogDataIndex, Name, this, UIPanelID.Quest);
        QuestManager.Instance.GiveChoice("Nomal", 3);
    }

    public override void Act()
    {
        base.Act();
        QuestManager.Instance.questWindow.OnClick_AcceptQuest();
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NPCTooltipUI : WorldTooltipUI
{
    [SerializeField] private TextMeshProUGUI npcNameText;
    [SerializeField] private TextMeshProUGUI interactionText;

    public void SetData(BaseNPC npc)
    {
        if (npc == null)
        {
            npcNameText.text = "";
            interactionText.text = "";
            return;
        }

        npcNameText.text = npc.Name;
        interactionText.text = "Talk";
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestUI : MonoBehaviour, IDialogSidePanel
{
    [SerializeField] private GameObject dialogPanel;
    [SerializeField] private GameObject[] panels;

    public GameObject[] Panel => panels;

    [SerializeField] private DialogUI dialogUI;
    [SerializeField] private QuestWindow questWindow;

    public void OnDialogLine(string npcName, string dialog, DialogChoice[] choices)
    {
        dialogUI.ShowSentence(npcName, dialog, choices);
    }

    public void OnDialogEnd(int defaultNextData)
    {
        questWindow.OpenQuestLists();
    }

    public void Show()
    {
        dialogPanel.SetActive(true);
    }

    public void Hide()
    {
        dialogPanel.SetActive(false);
        foreach (var p in panels)
            p.SetActive(false);
    }
}

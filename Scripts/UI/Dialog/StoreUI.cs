using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreUI : MonoBehaviour, IDialogSidePanel
{
    [SerializeField] private GameObject dialogPanel;

    [SerializeField] private GameObject[] panels;
    public GameObject[] Panel => panels;

    [SerializeField] private DialogUI dialogUI;
    [SerializeField] private StorePanel store;

    void Start()
    {
        Hide();
    }

    public void OnDialogLine(string npcName, string dialog, DialogChoice[] choices)
    {
        dialogPanel.SetActive(true);
        dialogUI.ShowSentence(npcName, dialog, choices);
    }

    public void OnDialogEnd(int defaultNextData)
    {
        Hide();
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

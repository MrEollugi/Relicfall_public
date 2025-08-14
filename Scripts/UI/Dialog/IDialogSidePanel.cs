using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDialogSidePanel
{
    GameObject[] Panel { get; }

    void Show();

    void Hide();
    
    void OnDialogLine(string npcName, string dialog, DialogChoice[] choices);

    void OnDialogEnd(int defaultNextData);
}

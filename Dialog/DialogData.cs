using System;
using UnityEngine;


[CreateAssetMenu(fileName = "DialogData", menuName = "Scriptable Objects/Data/Dialog Data")]
public class DialogData : ScriptableObject
{
    public DialogElement[] dialogElements;
    public int defaultNextData = -1; // 다음 Data 식별용 (-1 종료) 
}


[System.Serializable]
public class DialogElement
{
    public string dialog;
    public DialogChoice[] choices;  // 0은 그냥 대사, 1부터 선택지
}

[System.Serializable]
public class DialogChoice
{
    public string choiceText;
    public int nextDataIndex;
    public int panelIndex;
    public int resultCode;
}

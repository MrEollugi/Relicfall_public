using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestSaveData
{
    public string ID;
    public float curProgress;
    public QuestStatus status;
    public bool isClear;
    public bool isRewarded;


    public QuestSaveData()
    {
        ID = string.Empty;
        curProgress = 0f;
        status = QuestStatus.NotStarted;
        isClear = false;
        isRewarded = false;
    }
}

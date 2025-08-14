using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;

[System.Serializable]
public class QuestInstance  // 퀘스트 상태와 판정만 관리
{
    public QuestListSO Data;
    public QuestType Type;
    public QuestStatus Status { get; private set; }
    public bool IsRewarded { get; private set; }
    public float curProgress { get; private set; }
    public float maxProgress = 1f;

    // 퀘스트 완료/실패 이벤트
    public event Action<QuestInstance> OnSuccess;
    public event Action<QuestInstance> OnFail;

    public QuestInstance(QuestListSO questData)
    {
        Data = questData;
        Type = questData.questType;
        maxProgress = questData.requiredProgress;
        curProgress = 0f;
        Status = QuestStatus.NotStarted;
    }

    public void QuestStart() // 퀘스트를 시작 상태로 전환
    {
        Status = QuestStatus.InProgress;
    }

    public void QuestUpdateProgress(float value) // 진행도 업데이트
    {
        if (Status != QuestStatus.InProgress)
        {
            Debug.Log("아직 시작 안 함 -UpdateProgress");
            return;
        }

        curProgress = value;

        if (CheckCompletion())
        {
            Status = QuestStatus.Success;
            OnSuccess?.Invoke(this);
        }

        // 실패 조건?
    }

    private bool CheckCompletion()
    {
        // 목표치를 넘었는가?
        return curProgress >= Data.requiredProgress;
    }

    public void MarkSuccess()
    {
        if (Status == QuestStatus.Success)
            return;

        Status = QuestStatus.Success;

        OnSuccess?.Invoke(this);
    }

    public void MarkFail()
    {
        if (Status == QuestStatus.Fail)
            return;

        Status = QuestStatus.Fail;

        OnFail?.Invoke(this);
    }

    // 보상 지급 완료했는가?
    public void MarkRewarded()
    {
        IsRewarded = true;
    }

    // public void CancleQuest()
    // {
    //     status = QuestStatus.NotStarted;
    // }

    // public void QuestCheckSuccess()//던전? 클리어시 진행도 확인 후 결과
    // {
    //     switch (questData.questType)
    //     {
    //         case QuestType.ExplorationProgress:
    //             if(curProgress >= maxProgress * 0.01f)
    //             {
    //                 QuestSuccess();
    //             }
    //             return;
    //         case QuestType.BattleProgress:
    //             if (curProgress >= maxProgress)
    //             {
    //                 QuestSuccess();
    //             }
    //             return;
    //         case QuestType.BossClear:
    //             if (curProgress >= maxProgress)
    //             {
    //                 QuestSuccess();
    //             }
    //             return;
    //         case QuestType.RelicRecovery:
    //             if (curProgress >= maxProgress)
    //             {
    //                 QuestSuccess();
    //             }
    //             return;
    //     }

    //     QuestFail();
    // }

    // public void QuestSuccess()//퀘스트 성공 처리: 상태 변경, isClear = true
    // {
    //     status = QuestStatus.Success;
    //     isClear = true;
    //     QuestReward();
    // }

    // public void QuestFail()//퀘스트 실패 처리: 상태 변경
    // {
    //     status = QuestStatus.Faild;
    //     QuestPunish();
    // }

    // public void QuestReward()//퀘스트 완료시 보상지급
    // {
    //     //보상을 주고
    //     // 우선 골드만 지급
    //     GameManager.Instance.AddGold(questData.goldReward);
    //     isRewarded = true;
    // }
    // public void QuestPunish()//실패시 처벌
    // {

    // }

    // public void RefreshQuest(QuestListSO questData)
    // {
    //     this.questData = questData;
    //     curProgress = 0f;
    //     status = QuestStatus.NotStarted;
    //     isClear = false;
    //     isRewarded = false;
    // }

    public QuestSaveData GetQuestSaveData()
    {
        return new QuestSaveData
        {
            ID = Data.questID,
            curProgress = curProgress,
            status = Status,
            isClear = (Status == QuestStatus.Success),
            isRewarded = IsRewarded
        };
    }

    public void ApplyQuestSaveData(QuestSaveData questSaveData, List<QuestListSO> allQuestList)
    {
        var so = allQuestList.Find(q => q.questID == Data.questID);
        if (so == null)
        {
            Debug.Log($"'{questSaveData.ID}'를 가진 SO를 찾을 수 없음.-ApplyQuestSaveData()");
            return;
        }

        Data = so;
        curProgress = questSaveData.curProgress;
        Status = questSaveData.status;
        IsRewarded = questSaveData.isRewarded;
    }
}

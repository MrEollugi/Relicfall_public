using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Collections;
using UnityEngine;

public enum QuestType
{
    ExplorationProgress,//탐험 진행도
    BattleProgress,//배틀 진행
    BossClear,//보스 클리어
    RelicRecovery//유물 회수, 추후 추가?
}
public enum QuestArea//지역명?  일반, 왕국의뢰
{
    Nomal,
    Kingdom
}
public enum QuestStatus//퀘스트의 상태
{
    NotStarted,
    InProgress,
    Success,
    Fail
}

[CreateAssetMenu(fileName = "New Quest", menuName = "Quest/New Quest")]
public class QuestListSO : ScriptableObject//보상은 임의로 추가한것
{
    [Header("퀘스트 정보")]
    public string questID;  //  ID 예시?: "Q_KINGDOM_BATTLE_001"
    public string questName;
    [TextArea] public string description;
    public string targetID;

    [Header("퀘스트 분류")]
    public QuestType questType;
    public QuestArea questArea;
    public QuestStatus questStatus;

    [Header("퀘스트 상태")]
    public float curProgress;
    public float requiredProgress;
    public bool isClear;

    [Header("퀘스트 보상")]
    public int expReward;
    public int goldReward;
}
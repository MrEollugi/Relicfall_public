using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Quest Data", menuName = "Quest/Quest Data")]
public class QuestData : ScriptableObject//퀘스트들을 모아둘 리스트
{
    public List<QuestListSO> questLists;
}
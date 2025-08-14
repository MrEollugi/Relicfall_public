using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.SceneManagement;


/*

퀘스트 수락/진행/완료/실패 등을 처리

저장/로드, UI에 퀘스트 제공, 조건 확인 등도 포함

포함 항목 예시:

2. TMP랑 폰트 << 이거 안물어봄, 디자인 관련은 나중에 할거같으니 폰트는 미뤄두고

1. 왕국퀘스트란? 가 나오는 조건 - 나중
3. 보상 및 처벌 그 아래쪽 - 나중 디메리트 리스트같은거

4. 퀘스트 실패의 조건 - 
5. 스테이지 구성? 첫 지역 퀘스트를 클리어해야 다음지역이 열린다던가

6. 세이브는 json 맞음 << 추가하자

7. 다음에 할거 로비 화면 OR 사운드매니저 OR 옵션


//스테이지 구성?
//첫 지역 퀘스트를 한번은 클리어해야 다음지역 퀘스트가 열린다던가
//왕국 퀘스트는 무엇인가 이벤트 퀘스트 같은건가? 맞다면 조건도 있어야하고
//퀘스트 실패의 조건은 사망 말고 더 있나?


 */

public class QuestManager : Singleton<QuestManager> // 퀘스트 보상/벌칙, 저장, Scene 전환 처리 등
{
    public QuestWindow questWindow;
    public QuestData questData;

    [Header("전체 퀘스트 데이터")]
    public List<QuestListSO> allQuestList;

    [Header("현재 선택 가능한 퀘스트들")]
    public List<QuestListSO> canSelectQuest;


    [Header("현재 수락 중인 퀘스트")]
    [SerializeField] public QuestInstance currentQuest;

    [SerializeField] private MapManager mapManager;
    [SerializeField] private PlayerController controller;
    private InventoryGrid playerInventory;
    private int lastCollectedCount;

    protected override void Awake()
    {
        base.Awake();
        SceneManager.sceneLoaded += OnSceneLoaded;

        // SO에서 전체 퀘스트 목록 초기화
        allQuestList = new List<QuestListSO>(questData.questLists);

        // 저장된 Quest 불러오기
        // var world = GameManager.Instance.WorldData;
        // if (world.questSaveData != null && !string.IsNullOrEmpty(world.questSaveData.ID))
        // {
        //     var so = allQuestList.Find(q => q.questID == world.questSaveData.ID);
        //     if (so != null)
        //     {
        //         currentQuest = new QuestInstance(so);
        //         currentQuest.OnSuccess += HandleQuestSuccess;
        //         currentQuest.OnFail += HandleQuestFail;
        //         currentQuest.ApplyQuestSaveData(world.questSaveData, allQuestList);

        //         if (currentQuest.Status == QuestStatus.InProgress)
        //             currentQuest.QuestStart();
        //         else
        //             ProcessHubSceneEntry();
        //     }
        // }

        // controller = FindObjectOfType<PlayerController>();
        // if (controller != null)
        //     playerInventory = controller.Inventory;
    }

    private void OnDestroy()
    {
        UnsubscribeAll();

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private IEnumerator Start()
    {
        // QuestWindow 대기
        yield return new WaitUntil(() => UIManager.Instance.GetComponentInChildren<QuestWindow>(true) != null);
        questWindow = UIManager.Instance.GetComponentInChildren<QuestWindow>(true);
    }

    #region Quest Selection
    // 퀘스트를 받을 때 해당 지역(areaName)의 퀘스트를 count 개수 만큼
    public void GiveChoice(string areaName, int count)
    {
        canSelectQuest.Clear();
        List<QuestListSO> questListSOs = allQuestList.FindAll
            (q => q.questArea.ToString() == areaName && q.questStatus == QuestStatus.NotStarted);

        for (int i = 0; i < count; i++)
        {
            int random = Random.Range(0, questListSOs.Count);
            canSelectQuest.Add(questListSOs[random]);
            questListSOs.RemoveAt(random);
        }

        questWindow.OpenQuestLists();
    }

    public void CancelQuest()
    {
        if (currentQuest == null || currentQuest.Status != QuestStatus.InProgress)
        {
            Debug.Log("받은 퀘스트가 없음 - CancelQuest");
            //ui 
            return;
        }

        UnsubscribeAll();
        currentQuest = null;

        // var world = GameManager.Instance.WorldData;
        // world.questSaveData = new QuestSaveData();
        // GameManager.Instance.SaveCurrentState();

        questWindow.CloseQuestWindow();
        Debug.Log("Cancel Quest");
    }
    #endregion

    #region Quest Lifecycle
    public void StartQuest(QuestListSO selectedQuest) //퀘스트 시작 상태로 전환
    {
        if (currentQuest != null && currentQuest.Status == QuestStatus.InProgress)
        {
            Debug.Log("이미 진행 중인 퀘스트가 존재합니다");
            //ui팝업 연결
            return;
        }

        currentQuest = new QuestInstance(selectedQuest);
        currentQuest.QuestStart();

        currentQuest.OnSuccess += HandleQuestSuccess;
        currentQuest.OnFail += HandleQuestFail;

        Debug.Log($"현재 받은 퀘스트 {currentQuest.Data.questName} / {currentQuest.Data.description}\"");

        // 타입별 이벤트 구독
        switch (currentQuest.Type)
        {
            case QuestType.ExplorationProgress :
                if (mapManager != null) mapManager.OnNodeVisited += HandleNodeVisited;
                break;

            case QuestType.BattleProgress :
                EnemyEventChannel.OnEnemyKilled += HandleEnemyKilled;
                break;

            case QuestType.BossClear :
                EnemyEventChannel.OnBossKilled += HandleBossKilled;
                break;

            case QuestType.RelicRecovery :
                if (playerInventory != null) playerInventory.OnInventoryChanged += HandleInventoryChanged;
                break;
        }
    }

    private void UnsubscribeAll()
    {
        if (currentQuest != null)
        {
            currentQuest.OnSuccess -= HandleQuestSuccess;
            currentQuest.OnFail -= HandleQuestFail;
        }

        EnemyEventChannel.OnEnemyKilled -= HandleEnemyKilled;
        EnemyEventChannel.OnBossKilled -= HandleBossKilled;

        if (playerInventory != null)
            playerInventory.OnInventoryChanged -= HandleInventoryChanged;

        if (mapManager != null)
            mapManager.OnNodeVisited -= HandleNodeVisited;
    }

    public void UpdateProgress(float value) // 퀘스트 진행도 업데이트
    {
        if (currentQuest == null)
        {
            Debug.Log("현재 퀘스트 없음 - UpdateProgress");
            return;
        }

        currentQuest.QuestUpdateProgress(value);
    }
    #endregion

    #region Register
    public void RegisterMapManager(MapManager map)
    {
        mapManager = map;

        if (currentQuest != null && currentQuest.Type == QuestType.ExplorationProgress)
            mapManager.OnNodeVisited += HandleNodeVisited;
    }

    public void RegisterPlayerController(PlayerController con)
    {
        controller = con;
    }

    public void RegisterInventory(InventoryGrid inventory)
    {
        playerInventory = inventory;
        
        if (currentQuest != null && currentQuest.Type == QuestType.RelicRecovery)
            playerInventory.OnInventoryChanged += HandleInventoryChanged;
    }
    #endregion

    #region Event Handler
    private void HandleEnemyKilled(string EnemyName)
    {
        if (currentQuest == null || currentQuest.Type != QuestType.BattleProgress)
            return;

        UpdateProgress(currentQuest.curProgress + 1);
    }

    private void HandleBossKilled(string bossName)
    {
        if (currentQuest == null || currentQuest.Type != QuestType.BossClear)
            return;

        // cur + 1? max?
        UpdateProgress(currentQuest.curProgress + 1);
    }

    private void HandleInventoryChanged()
    {
        if (currentQuest == null || currentQuest.Type != QuestType.RelicRecovery)
            return;

        Debug.Log("Collect Item");

        var items = playerInventory.GetAllItems();
        int count = items.Count(i => i.data.id == currentQuest.Data.targetID);

        if (count != lastCollectedCount)
        {
            lastCollectedCount = count;
            UpdateProgress(count);

            if (count >= currentQuest.maxProgress)
                currentQuest.MarkSuccess();
        }
    }

    private void HandleNodeVisited(MapNode node)
    {
        if (currentQuest == null || currentQuest.Data.questType != QuestType.ExplorationProgress)
            return;

        float visitedCount = mapManager.GetVisitedNodeCount();
        float totalCount = mapManager.GetTotalNodeCount();
        float progress = visitedCount / totalCount;

        Debug.Log($"[탐험 퀘스트 진행] " + $"방문: {visitedCount} / {totalCount} ({progress:P1})");

        UpdateProgress(progress);
    }

    private void HandleQuestSuccess(QuestInstance quest)
    {
        if (SceneManager.GetActiveScene().name != "HubScene")
            return;

        GiveReward(quest);
        quest.MarkRewarded();
        SaveQuestState();
        UnsubscribeAll();
    }

    private void HandleQuestFail(QuestInstance quest)
    {
        if (SceneManager.GetActiveScene().name != "HubScene")
            return;
            
        GivePenalty(quest);
        quest.MarkRewarded();
        SaveQuestState();
        UnsubscribeAll();
    }
    #endregion

    #region Scene Load & Rewards
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (controller != null)
            RegisterInventory(playerInventory);

        if (scene.name == "HubScene")
        {
            ProcessHubSceneEntry();
        }
    }

    private void ProcessHubSceneEntry()
    {
        if (currentQuest == null || currentQuest.IsRewarded)
            return;

        switch (currentQuest.Status)
        {
            case QuestStatus.Success:
                HandleQuestSuccess(currentQuest);
                break;

            case QuestStatus.Fail:
                HandleQuestFail(currentQuest);
                break;
        }
    }

    private void GiveReward(QuestInstance quest)
    {
        GameManager.Instance.AddGold(quest.Data.goldReward);
        // 이후 다른 보상 (아이템)
        // Reward UI
        Debug.Log($"퀘스트 보상: {quest.Data.goldReward} G 지급");
    }

    private void GivePenalty(QuestInstance quest)
    {
        Debug.Log("퀘스트 실패: Give Penalty");
    }

    private void SaveQuestState()
    {
        if (currentQuest == null)
            return;

        var world = GameManager.Instance.WorldData;
        world.questSaveData = currentQuest.GetQuestSaveData();
        GameManager.Instance.SaveCurrentState();
    }
    #endregion
}
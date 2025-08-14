using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class QuestWindow : MonoBehaviour//퀘스트 창
{
    // public QuestManager questManager;
    [SerializeField] private QuestListSO selectedQuest;

    [SerializeField] private TextMeshProUGUI QuestName;
    [SerializeField] private TextMeshProUGUI QuestDescription;
    [SerializeField] private TextMeshProUGUI QuestID;
    [SerializeField] private TextMeshProUGUI QuestReward;
    [SerializeField] private TextMeshProUGUI QuestProgress;

    public GameObject GOQuestWindow;//퀘스트 받은거 확인할곳
    public GameObject GOQuestLists;//퀘스트 리스트 뽑아주는거 고르는곳
    public GameObject questButtonPrefab; 
    public Transform questButtonParent;  // 프리펩이 만들어질 위치
    public GameObject popupIsAccept;
    public GameObject pupupIsCancle;
    public GameObject AcceptButton;

    public GameObject QuestUI;
    [SerializeField] private List<GameObject> listButtons;

    public void SetQuestBtn()
    {
        foreach (var b in listButtons)
        {
            if (b != null) Destroy(b);
        }

        listButtons.Clear();

        var quests = QuestManager.Instance.canSelectQuest;

        for (int i = 0; i < QuestManager.Instance.canSelectQuest.Count; i++)
        {
            int index = i;
            QuestListSO questListSo = quests[index];

            GameObject buttonObj = Instantiate(questButtonPrefab, questButtonParent);
            listButtons.Add(buttonObj);

            TMP_Text buttonText = buttonObj.GetComponentInChildren<TMP_Text>();
            buttonText.text = $"{questListSo.name}\n{questListSo.description}";

            Button btn = buttonObj.GetComponent<Button>();
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => OpenQuestWindow(quests[index]));
        }

        AcceptButton.SetActive(true);
    }


    public void OpenIsAccept()
    {
        //selectedQuest = questManager.canSelectQuest[index];

        popupIsAccept.SetActive(true);
    }

    public void OnClick_AcceptQuest()
    {
        if (selectedQuest == null)
        {
            Debug.LogWarning("선택된 퀘스트 없음!");
            return;
        }

        IsAccept(selectedQuest); // 기억해뒀던 퀘스트 전달

        QuestManager.Instance.canSelectQuest.Remove(selectedQuest);

        popupIsAccept.SetActive(false); // 팝업 닫기
        // window도 닫고 마지막 dialog 후 대화 종료하고 싶은데
        GOQuestWindow.SetActive(false);
        GOQuestLists.SetActive(false);

        // SetQuestBtn();
    }

    public void CloseIsAccept()
    {
        popupIsAccept.SetActive(false);
    }
    
    public void IsAccept(QuestListSO selectedQuest)//수락?
    {
        QuestManager.Instance.StartQuest(selectedQuest);
    }

    public void OpenIsCancle()
    {
        pupupIsCancle.SetActive(true);
    }

    public void CloseIsCancle()
    {
        pupupIsCancle.SetActive(false);
    }

    public void IsCancle()
    {
        QuestManager.Instance.CancelQuest();    // 퀘스트 취소
        pupupIsCancle.SetActive(false);         // Popup 닫기
        RefreshQuest();                         // 퀘스트 window 리셋
        SetQuestBtn();                          // Quest List reset
        GOQuestWindow.SetActive(false);         // Close QW
        GOQuestLists.SetActive(true);           // Open QL
    }

    public void RefreshQuest()
    {
        if(QuestManager.Instance.currentQuest == null)
        {
            QuestName.text = "Name";
            QuestDescription.text = "Description";
            QuestID.text = "ID";
            QuestReward.text = "Reward";
            QuestProgress.text = "Required Progress";
        }
        else
        {
            QuestName.text = QuestManager.Instance.currentQuest.Data.questName;
            QuestDescription.text = QuestManager.Instance.currentQuest.Data.description;
            QuestID.text = QuestManager.Instance.currentQuest.Data.questID;
            QuestReward.text = QuestManager.Instance.currentQuest.Data.goldReward.ToString()
                                + QuestManager.Instance.currentQuest.Data.expReward;
            QuestProgress.text = QuestManager.Instance.currentQuest.maxProgress.ToString();
        }
    }

    public void DisplayQuestDescription()
    {
        Debug.Log("Display Quest Description");
        QuestName.text = selectedQuest.questName;
        QuestDescription.text = selectedQuest.description;
        QuestID.text = selectedQuest.questID;
        QuestReward.text = selectedQuest.goldReward.ToString()
                            + selectedQuest.expReward;
        QuestProgress.text = selectedQuest.curProgress.ToString();
    }

    public void OpenQuestWindow(QuestListSO quest)
    {
        if (QuestManager.Instance.currentQuest != null
        && QuestManager.Instance.currentQuest.Status == QuestStatus.InProgress)
        {
            AcceptButton.SetActive(false);
        }
        else
        {
            AcceptButton.SetActive(true);
        }

        Debug.Log("Open Quest Window Method");
        selectedQuest = quest;
        // QuestInstance questInstance = new QuestInstance(selectedQuest);
        // QuestManager.Instance.currentQuest = questInstance;
        // RefreshQuest();
        DisplayQuestDescription();
        GOQuestWindow.SetActive(true);
    }

    public void CloseQuestWindow()
    {
        if (QuestManager.Instance.currentQuest != null
            && QuestManager.Instance.currentQuest.Status == QuestStatus.InProgress)
        {
            DialogSystem.Instance.FinishDialog();
            return;
        }
        GOQuestWindow.SetActive(false);
    }

    public void OpenQuestLists()
    {
        if (QuestManager.Instance.currentQuest != null
            && QuestManager.Instance.currentQuest.Status == QuestStatus.InProgress)
        {
            // GOQuestLists.SetActive(false);
            // GOQuestWindow.SetActive(true);
            OpenQuestWindow(QuestManager.Instance.currentQuest.Data);
        }
        else
        {
            SetQuestBtn();
            GOQuestLists.SetActive(true);
            GOQuestWindow.SetActive(false);
        }
    }
    public void CloseQuestLists()
    {
        // GOQuestLists.SetActive(false);
        // QuestUI.SetActive(false);
        DialogSystem.Instance.FinishDialog();
    }

}

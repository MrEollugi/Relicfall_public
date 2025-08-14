using System;
using System.Collections;
using UnityEngine;

public class DialogSystem : Singleton<DialogSystem>
{
    public DialogData[] dialogDatas;
    private DialogData curData;
    private int lineIndex;
    private BaseNPC currentNPC;

    private UIPanelID currentPanelID;

    [SerializeField] private PlayerController player;

    private IDialogSidePanel sidePanel;

    protected override void Awake()
    {
        base.Awake();
        // dialogUI = GetComponent<DialogUI>();

    }

    public void StartDialog(int dataIndex, string npcName, BaseNPC npc, UIPanelID id)
    {
        player = FindObjectOfType<PlayerController>();

        // 인덱스 오류 예외 처리
        if (dataIndex < 0 || dataIndex >= dialogDatas.Length) return;

        currentNPC = npc;
        currentPanelID = id;
        curData = dialogDatas[dataIndex];
        lineIndex = 0;

        // Player 이동 제한? 같은 거? 여기서 하면 될 듯?
        player.BlockInput();

        UIManager.Instance.ShowPanel(id);

        var sPanel = UIManager.Instance.GetPanelGO(id);
        sidePanel = sPanel.GetComponentInChildren<IDialogSidePanel>(true);

        StartCoroutine(DisplayCurrentLine());
    }

    // 다음 문장 있는지 확인
    public void OnSentenceFinished(string npcName)
    {
        if (lineIndex < curData.dialogElements.Length)
        {
            StartCoroutine(DisplayCurrentLine());
        }
        else
        {
            EndOrNextData(npcName, curData.defaultNextData);
        }
    }

    // 선택지
    public void OnChoiceSelected(string npcName, DialogChoice choice)
    {
        Debug.Log($"버튼 눌림{npcName}{choice}");
        EndOrNextData(npcName, choice.nextDataIndex);

        sidePanel.Hide();

        if (choice.panelIndex >= 0 && choice.panelIndex < sidePanel.Panel.Length)
        {
            var go = sidePanel.Panel[choice.panelIndex];
            go.SetActive(true);
        }

        HandleChoiceResult(choice.resultCode);
    }

    // 현재 문장 Dialog 출력
    private IEnumerator DisplayCurrentLine()
    {
        if (curData.dialogElements == null || lineIndex >= curData.dialogElements.Length)
        {
            EndOrNextData(currentNPC.name, curData.defaultNextData);
            yield break;
        }

        yield return new WaitUntil(() => UIManager.Instance.GetPanelGO(currentPanelID).activeInHierarchy); // dialogUI.gameObject.activeInHierarchy || storeUI.gameObject.activeInHierarchy);

        var line = curData.dialogElements[lineIndex++];

        sidePanel.OnDialogLine(currentNPC.name, line.dialog, line.choices);
    }

    // Data 끝에서 판별
    private void EndOrNextData(string npcName, int nextDataIndex)
    {
        if (nextDataIndex < 0)
        {
            FinishDialog();
            return;
        }

        if (nextDataIndex >= dialogDatas.Length)
        {
            FinishDialog();
            return;
        }

        curData = dialogDatas[nextDataIndex];
        lineIndex = 0;
        StartCoroutine(DisplayCurrentLine());
    }

    public void FinishDialog()
    {
        sidePanel.OnDialogEnd(curData.defaultNextData);

        UIManager.Instance.HidePanel(UIPanelID.Dialogue);
        UIManager.Instance.HidePanel(currentPanelID);

        // Player 이동 제한 해제?
        player.UnblockInput();
        currentNPC?.DialogFinished();
        // currentNPC = null;
        Debug.Log("FinishDialog");
    }

    private void HandleChoiceResult(int code)
    {
        switch (code)
        {// Choice 코드에 따른 로직 실행

            case 0:
                break;

            case 1:
                sidePanel.Show();
                break;

            case 2:
                currentNPC?.Act();
                break;
        }
    }
}

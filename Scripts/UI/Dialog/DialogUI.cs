using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI npcNameText;
    [SerializeField] private TextMeshProUGUI dialogText;
    [SerializeField] private float typeSpeed = 0.04f;

    [SerializeField] private Transform choiceRoot;
    [SerializeField] private Button[] staticChoiceButtons;
    [SerializeField] private Button choicePrefab;

    private readonly List<Button> dynamicButtons = new List<Button>();

    private bool isTyping;
    private string curSentence;

    public PlayerController player;
    private InputManager inputManager;

    void Start()
    {
        player = FindObjectOfType<PlayerController>();
    }

    void Update()
    {
        inputManager = player.InputManager;

        inputManager.UpdateInputData();
        InputData input = inputManager.GetInput();

        if (!gameObject.activeSelf) return;

        if (input.isInteractDown)
        {
            if (isTyping)
            {
                StopAllCoroutines();
                dialogText.text = curSentence;
                isTyping = false;
            }
            else
            {
                DialogSystem.Instance.OnSentenceFinished(npcNameText.text);
            }
        }
    }

    public void ShowSentence(string npcName, string dialog, DialogChoice[] choices)
    {
        StopAllCoroutines();
        isTyping = false;
        dialogText.text = "";

        foreach (var b in staticChoiceButtons)
        {
            b.onClick.RemoveAllListeners();
            b.gameObject.SetActive(false);
        }

        foreach (var b in dynamicButtons)
        {
            Destroy(b.gameObject);
        }
        dynamicButtons.Clear();

        if (!gameObject.activeSelf)
            gameObject.SetActive(true);

        npcNameText.text = npcName;
        curSentence = dialog;
        
        StartCoroutine(TypeWriter(choices));

        bool hasChoices = choices != null && choices.Length > 0;
        choiceRoot.gameObject.SetActive(hasChoices);

        if (hasChoices)
        {
            BuildChoiceButtons(npcName, choices);
        }
    }

    private IEnumerator TypeWriter(DialogChoice[] choices)
    {
        isTyping = true;
        dialogText.text = "";

        foreach (var a in curSentence)
        {
            dialogText.text += a;
            yield return new WaitForSeconds(typeSpeed);
        }

        isTyping = false;
    }

    private void BuildChoiceButtons(string npcName, DialogChoice[] choices)
    {
        // Debug.Log("BuildChoiceButtons 시작");

        // foreach (Transform child in choiceRoot)
        // {
        //     Destroy(child.gameObject);
        // }

        // Debug.Log("BuildChoiceButtons시작");
        // foreach (var c in choices)
        // {
        //     Debug.Log($"{c}버튼 생성");
        //     Button button = Instantiate(choicePrefab, choiceRoot);
        //     button.GetComponentInChildren<TextMeshProUGUI>().text = c.choiceText;
        //     button.onClick.AddListener(() =>
        //     {
        //         DialogSystem.Instance.OnChoiceSelected(npcName, c);
        //         Debug.Log("버튼 만듬");
        //     });
        // }

        for (int i = 0; i < choices.Length; i++)
        {
            Button btn;

            if (i < staticChoiceButtons.Length)
            {
                btn = staticChoiceButtons[i];
            }
            else
            {
                btn = Instantiate(choicePrefab, choiceRoot);
                dynamicButtons.Add(btn);
            }

            btn.gameObject.SetActive(true);
            btn.GetComponentInChildren<TextMeshProUGUI>().text = choices[i].choiceText;

            int index = i;
            btn.onClick.AddListener(() => DialogSystem.Instance.OnChoiceSelected(npcName, choices[index]));
        }        
    }

}

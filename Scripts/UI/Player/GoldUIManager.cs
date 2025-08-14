using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GoldUIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text goldText;

    void Start()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogWarning("GoldUIManager: Start()에서도 GameManager.Instance가 null입니다.");
            return;
        }

        GameManager.Instance.OnGoldChanged += HandleGoldChanged;

        UpdateGoldUI();
    }

    void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnGoldChanged -= HandleGoldChanged;
    }
    void HandleGoldChanged(int newGold)
    {
        goldText.text = $"{newGold}";
    }

    public void UpdateGoldUI()
    {
        int gold = GameManager.Instance.WorldData.TotalGold;
        //goldText.text = $"{gold:N0}"; // 1,000 G 식 포맷
        goldText.text = $"{gold}";
    }
}

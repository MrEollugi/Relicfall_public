using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Store : MonoBehaviour
{
    SellZone sellZone;
    public int reward;
   
    private void Start()
    {
        sellZone = GetComponent<SellZone>();
    }

    public void OpenStore()
    {
        // UIManager.Instance. ... UI 열기
        Debug.Log("Open Store");
    }

    public int CalReward()
    {
        return reward = sellZone.ItemReward();
    }

    public void SellItem()
    {
        CalReward();
        sellZone.ItemSell();

        // 골드는 반드시 GameManager를 통해 추가!
        GameManager.Instance.AddGold(reward);
    }



}


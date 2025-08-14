using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SellZone : MonoBehaviour
{

    public List<GameObject> SellList = new List<GameObject>();

    private void Update()
    {
        SellList.RemoveAll(item => item == null);
    }

    private void FixedUpdate()
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log($"{other}들어옴");

        if(other.CompareTag("Item"))
        {
            SellList.Add(other.gameObject);
            Debug.Log($"{other} 판매리스트에 추가");
        }
        else
        {
            return;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        Debug.Log($"{other}나감");

        if (other.CompareTag("Item"))
        {
            SellList.Remove(other.gameObject);
            
            Debug.Log($"{other} 판매리스트에서 삭제");
        }
    }

    public int ItemReward()
    {
        int reward = 0;

        foreach(var obj in SellList)
        {
            var item = obj.GetComponent<Item>();
            if (item == null)
                continue;

            ItemInstance itemInstance = item.GetData();

            int range = itemInstance.data.valueRange;

            var itemValue = itemInstance.ActualValue * itemInstance.stackCount;

            reward += itemValue;
        }

        return reward;
    }

    public void ItemSell()
    {
        SellList.RemoveAll(item => item == null);

        foreach (var obj in SellList)
        {
            Destroy(obj);
        }

        SellList.Clear();
    }

}

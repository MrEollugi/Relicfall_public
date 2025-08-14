using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public string pairId;
    private Door destinationDoor;

    private void Start()
    {
        Door[] allDoors = FindObjectsOfType<Door>();
        foreach (var d in allDoors)
        {
            if (d != this && d.pairId == this.pairId)
            {
                destinationDoor = d;
                break;
            }
        }
    }

    void OnTriggerEnter(Collider collision)
    {
        if (destinationDoor == null)
        {
            Door[] allDoors = FindObjectsOfType<Door>();
            foreach (var d in allDoors)
            {
                if (d != this && d.pairId == this.pairId)
                {
                    destinationDoor = d;
                    break;
                }
            }
        }
        if (collision.gameObject.layer == 3 && destinationDoor != null)
        {
            // 문 위치에서 z만 -5 떨어진 위치로 이동
            Vector3 targetPos = destinationDoor.transform.position;
            targetPos.z -= 3f;

            collision.transform.position = targetPos;
        }
    }

    public void LinkPair(IEnumerable<Door> allDoors)
    {
        foreach (var d in allDoors)
        {
            if (d != this && d.pairId == this.pairId)
            {
                destinationDoor = d;
                return;
            }
        }
        Debug.LogWarning("짝 문을 찾을 수 없습니다: " + pairId);
    }

}

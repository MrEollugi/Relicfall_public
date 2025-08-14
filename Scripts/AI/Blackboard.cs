using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blackboard : MonoBehaviour
{
    [Header("References")]
    public Transform Player;

    [Header("State")]

    // public ItemSlot StolenItem;

    [Header("Needs")]
    public float inTime;
    public bool isPlayerIn;

    [Header("Perception")]
    public Vector3 lastHeardPosition;
    public float lastHeardTime;
    public bool IsPlayerVisible;
    public float LastHitTime;

    public bool isInvestigating;


    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    void Start()
    {
        // 1. GameManager 싱글톤이 있을 경우
        if (GameManager.Instance != null && GameManager.Instance.CurrentPlayer != null)
        {
            Player = GameManager.Instance.CurrentPlayer.transform;
        }
        // 2. PlayerController가 씬에 있다면
        else
        {
            // 가장 일반적인 방식: 씬에 실제로 존재하는 PlayerController를 찾는다
            var playerObj = FindObjectOfType<PlayerController>();
            if (playerObj != null)
                Player = playerObj.transform;
        }
    }
}

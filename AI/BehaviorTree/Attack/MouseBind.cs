using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


//개선해야할거? 다른씬에도 페이크마우스 보이게 하기, 플레이어가 페이크마우스를 바라보게하기,몬스터 공통으로 몬스터가 죽으면 패턴이 즉시 사라지는지 확인해야함
//씬 이동시 에러뜨는지 확인하기,


public class MouseBind : MonoBehaviour//마우스(가짜 ui)가 몬스터 방향에 묶이는 스크립트
{
    public float inTime;
    private float time;

    private Blackboard bb;

    [SerializeField]private FakeCursor cursor;


    private void Awake()
    {
        bb = GetComponent<Blackboard>();

        if(cursor == null )
            cursor = FindFirstObjectByType<FakeCursor>();

    }
    private void Update()
    {
        if (bb.isPlayerIn == true)//플레이어가 특정 구역에 들어오면 시간증가, 그 시간이 SO에서 정한 시간을 넘는다면 마우스를 이 오브젝트의 위치로 고정
        {
            //Debug.Log("들어옴?");
            inTime += Time.deltaTime;

            if(inTime >= bb.inTime)
            {
                //Debug.Log("마우스 바인드 실행");
                cursor.isMovable = false;

                Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position); // 이 오브젝트의 화면 좌표 구하기

                cursor.SetCursorPosition(screenPos); // FakeCursor를 해당 화면 좌표로 이동
            }
        }
        else if(bb.isPlayerIn == false)//플레이어가 해당 오브젝트 주변에서 벗어나면 시간감소
        {
            //Debug.Log("나감?");
            cursor.isMovable = true;

            if(inTime > 0)
            {
                inTime -= Time.deltaTime * 2;
            }
            else
            {
                inTime = 0;
            }
        }


    }
}
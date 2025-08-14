using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeCursor : MonoBehaviour//인게임에서 대신 사용?될 마우스의 스크립트. 그냥 이미지고 클릭및 방향 전환 등은 안보이는 마우스가 함
{
    public bool isMovable = true;//true면 가짜커서가 윈도우 커서의 움직임을 따라감
    //몹 기믹 등이 있으면 false,
    public Vector2 mousePosition = new Vector2(0f, -20f);


    private bool isCursor = true;
    private RectTransform rectTransform;
    private RectTransform parentRectTransform;

    private void Awake()//시작시 윈도우마우스 끄기, 현재는 z키로 마우스 껐다켰다 가능
    {
        Cursor.visible = false;

        rectTransform = GetComponent<RectTransform>();
        parentRectTransform = transform.parent as RectTransform;

    }

    void CursorView()
    {
        if (isCursor)
        {
            isCursor = !isCursor;
            Cursor.visible = true;
        }
        else
        {
            isCursor = !isCursor;
            Cursor.visible = false;
        }
    }

    public void SetCursorPosition(Vector3 screenPos)
    {
        if (parentRectTransform == null) return;

        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRectTransform, screenPos, null, out localPos);

        rectTransform.anchoredPosition = localPos + mousePosition;
    }

    void Update()
    {
        if (isMovable)
        {
            SetCursorPosition(Input.mousePosition);
            //transform.position = Input.mousePosition;   
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            CursorView();
        }

    }
}

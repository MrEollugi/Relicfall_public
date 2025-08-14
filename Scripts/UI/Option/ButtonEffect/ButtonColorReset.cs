using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonColorReset : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        // 버튼이 선택되어 있다면 초기화
        if (ButtonEffect.currentlySelected != null)
        {
            ButtonEffect.currentlySelected.Deselect();
            ButtonEffect.currentlySelected = null;
        }
    }
}

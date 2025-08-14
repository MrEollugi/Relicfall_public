using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BtnClick : MonoBehaviour//버튼에 이 스크립트를 붙이고 사운드만 넣어주면 버튼 클릭시 사운드 출력
{
    public AudioClip btnClip;
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() => SoundManager.Instance.PlaySFX(btnClip, 0f));
    }
}

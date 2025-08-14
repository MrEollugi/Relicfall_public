using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class SoundOption : MonoBehaviour
{
    public Slider masterSlider;
    public Slider bgmSlider;
    public Slider sfxSlider;

    public GameObject soundOption;
    public TextMeshProUGUI curMasterVolume;
    public TextMeshProUGUI curBgmVolume;
    public TextMeshProUGUI curSfxVolume;

    private Dictionary<SoundType, bool> muteStates = new();
    private Dictionary<SoundType, float> savedVolumes = new();

    void Start()
    {
        // 슬라이더 값이 바뀔 때 OnSliderValueChanged 실행
        //masterSlider.value = SoundManager.Instance.masterVolume;
        //curMasterVolume.text = (SoundManager.Instance.masterVolume * 100).ToString("F0");
        //bgmSlider.value = SoundManager.Instance.bgmVolume;
        //curBgmVolume.text = (SoundManager.Instance.bgmVolume * 100).ToString("F0");
        //sfxSlider.value = SoundManager.Instance.sfxVolume;
        //curSfxVolume.text = (SoundManager.Instance.sfxVolume * 100).ToString("F0");

        //masterSlider.onValueChanged.AddListener((value) => OnSliderChanged(master, value));
        //bgmSlider.onValueChanged.AddListener((value) => OnSliderChanged(bgm, value));
        //sfxSlider.onValueChanged.AddListener((value) => OnSliderChanged(sfx, value));

        foreach (SoundType type in System.Enum.GetValues(typeof(SoundType)))
        {
            float volume = GetInitialVolumeByType(type);

            muteStates[type] = false;
            savedVolumes[type] = volume;

            Slider slider = GetSliderByType(type);
            TextMeshProUGUI volumeText = GetVolumeTextByType(type);

            slider.value = volume;
            volumeText.text = (volume * 100).ToString("F0");

            slider.onValueChanged.AddListener((v) => OnSliderChanged(type, v));
        }
    }

    #region 사운드 타입 관련
    private float GetInitialVolumeByType(SoundType type)
    {
        return type switch
        {
            SoundType.Master => SoundManager.Instance.masterVolume,
            SoundType.Bgm => SoundManager.Instance.bgmVolume,
            SoundType.Sfx => SoundManager.Instance.sfxVolume,
            _ => 0
        };
    }

    private Slider GetSliderByType(SoundType type)
    {
        return type switch
        {
            SoundType.Master => masterSlider,
            SoundType.Bgm => bgmSlider,
            SoundType.Sfx => sfxSlider,
            _ => null
        };
    }

    private TextMeshProUGUI GetVolumeTextByType(SoundType type)
    {
        return type switch
        {
            SoundType.Master => curMasterVolume,
            SoundType.Bgm => curBgmVolume,
            SoundType.Sfx => curSfxVolume,
            _ => null
        };
    }

    private void SetVolumeByType(SoundType type, float value)
    {
        switch (type)
        {
            case SoundType.Master:
                SoundManager.Instance.SetMasterVolume(value);
                break;
            case SoundType.Bgm:
                SoundManager.Instance.SetBgmVolume(value);
                break;
            case SoundType.Sfx:
                SoundManager.Instance.SetSfxVolume(value);
                break;
        }
    }

    private float GetDefaultVolumeByType(SoundType type)
    {
        return type switch
        {
            SoundType.Master => SoundManager.DEFAULT_MASTER_VOLUME,
            SoundType.Bgm => SoundManager.DEFAULT_BGM_VOLUME,
            SoundType.Sfx => SoundManager.DEFAULT_SFX_VOLUME,
            _ => 0.5f
        };
    }
    #endregion

    public void OpenSoundOption()
    {
        soundOption.SetActive(true);
    }

    public void CloseSoundOption()
    {
        soundOption.SetActive(false);
    }

    void OnSliderChanged(SoundType type, float value)//슬라이더 변경이 감지됬을때 거기에 맞춰 변경
    {
        //if(type == "Bgm")
        //{
        //    if (!isBgmMute)
        //    {
        //        isBgmMute = false;

        //        SoundManager.Instance.SetBgmVolume(value);
        //        curBgmVolume.text = (SoundManager.Instance.bgmVolume * 100).ToString("F0");

        //    }
        //}
        //else if (type == "Sfx")
        //{
        //    if (!isSfxMute)
        //    {
        //        SoundManager.Instance.SetSfxVolume(value);
        //        curSfxVolume.text = (SoundManager.Instance.sfxVolume * 100).ToString("F0");
        //    }
        //}

        if (muteStates.ContainsKey(type) && muteStates[type]) return;

        SetVolumeByType(type, value);
        GetVolumeTextByType(type).text = (value * 100).ToString("F0");

        if (SoundManager.Instance != null)
            SoundManager.Instance.SaveVolumeSettings();
    }

    public void ResetSoundOptions()
    {
        if (SoundManager.Instance == null) return;

        SoundManager.Instance.ResetToDefaultVolume();

        foreach (SoundType type in Enum.GetValues(typeof(SoundType)))
        {
            float defaultVal = GetDefaultVolumeByType(type);
            GetSliderByType(type).value = defaultVal;
            GetVolumeTextByType(type).text = (defaultVal * 100).ToString("F0");
        }
    }

    public void MuteMasterBtn() => ToggleMute(SoundType.Master);
    public void MuteBgmBtn() => ToggleMute(SoundType.Bgm);
    public void MuteSfxBtn() => ToggleMute(SoundType.Sfx);

    public void ToggleMute(SoundType type)
    {
        muteStates[type] = !muteStates[type];

        Slider slider = GetSliderByType(type);
        TextMeshProUGUI volumeText = GetVolumeTextByType(type);

        if (muteStates[type]) // 음소거 ON
        {
            savedVolumes[type] = slider.value;

            SetVolumeByType(type, 0f);
            volumeText.text = "0";
        }
        else // 음소거 OFF
        {
            float restoredValue = savedVolumes[type];
            slider.value = restoredValue;
            OnSliderChanged(type, restoredValue);
        }

        ChangeSlider(slider, muteStates[type]);
    }

    void ChangeSlider(Slider slider, bool isMuted)//음소거 버튼을 누를시에 슬라이더의 투명도 조절
    {
        Image fill = slider.fillRect.GetComponent<Image>();
        Color c = fill.color;
        c.a = isMuted ? 0.1f : 1f;
        fill.color = c;

        slider.interactable = !isMuted;

        //Image bgmFill = bgmSlider.fillRect.GetComponent<Image>();
        //Image sfxFill = sfxSlider.fillRect.GetComponent<Image>();

        //if (type == "Bgm")
        //{
        //    Color c = bgmFill.color;
        //    if (isBgmMute == true)
        //    {
        //        c.a = 0.1f;
        //        bgmFill.color = c;
        //    }
        //    else
        //    {
        //        c.a = 1f;
        //        bgmFill.color = c;
        //    }
        //}
        //else if (type == "Sfx")
        //{
        //    Color c = sfxFill.color;
        //    if (isSfxMute == true)
        //    {
        //        c.a = 0.1f;
        //        sfxFill.color = c;
        //    }
        //    else
        //    {
        //        c.a = 1f;
        //        sfxFill.color = c;
        //    }
        //}
    }
}


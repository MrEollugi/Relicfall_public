using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem.XR;
using UnityEngine.SceneManagement;
//사용법? : Player나 Enermy등에 public AudioCLip attackClip; 같은거 넣고 인스펙터창에서 연결
//그리고 attack메서드에 SoundManager.Instance.PlaySFX(attackClip, 0.1f);

public enum SoundType
{
    Master,
    Bgm,
    Sfx
}

public class SoundManager : Singleton<SoundManager> 
{
    [Header("Volume")]
    [Range(0f, 1f)] public float masterVolume = 0.5f;
    [Range(0f, 1f)] public float sfxVolume = 1.0f;
    [Range(0f, 1f)] public float bgmVolume = 1.0f;

    [Header("Mixer")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioMixerGroup sfxMixerGroup;
    [SerializeField] private AudioMixerGroup hitSfxMixerGroup;

    [SerializeField] private GameObject soundPrefab;
    [SerializeField] private Transform soundParent;
    [SerializeField] private AudioSource bgmSource;

    private SoundPool soundPool;
    public SoundSource soundSource;

    public SoundPool hitSfxPool { get; private set; }

    public AudioClip test1;
    public AudioClip test2;
    public AudioClip lobbyBGM;
    public AudioClip test4;

    private string currentPlace = "";
    private AudioClip currentBGMClip;

    private const string MASTER_VOLUME_PARAM = "MasterVolume";
    private const string BGM_VOLUME_PARAM = "BGMVolume";
    private const string SFX_VOLUME_PARAM = "SFXVolume";

    public const float DEFAULT_MASTER_VOLUME = 0.5f;
    public const float DEFAULT_BGM_VOLUME = 1.0f;
    public const float DEFAULT_SFX_VOLUME = 1.0f;

    private bool isSoundOn = true;

    protected override void Awake()
    {
        base.Awake();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void Start()
    {
        soundPool = new SoundPool(soundPrefab, soundParent, sfxMixerGroup);
        hitSfxPool = new SoundPool(soundPrefab, soundParent, hitSfxMixerGroup);


        LoadVolumeSettings();
        
        PlayCurPlaceBGM(SceneManager.GetActiveScene().name);
    }
    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Z))
    //    {
    //        Debug.Log("test1");
    //        PlaySFX(test1, 0.1f);
    //    }

    //    if (Input.GetKeyDown(KeyCode.X))
    //    {
    //        Debug.Log("test2");
    //        PlaySFX(test2, 0.1f);
    //    }

    //    if (Input.GetKeyDown(KeyCode.C))
    //    {
    //        Debug.Log("test3");
    //        PlayCurPlaceBGM("testScene");
    //    }

    //    if (Input.GetKeyDown(KeyCode.V))
    //    {
    //        Debug.Log("test4");
    //        PlayCurPlaceBGM("testScene1");
    //    }
    //}

    private void PlayCurPlaceBGM(string curPlace)
    {
        if (curPlace == currentPlace) return;

        currentPlace = curPlace;

        AudioClip clip = null;

        switch (curPlace)
        {
            case "LobbyScene":
                clip = lobbyBGM;
                break;
            case "Test_Player":
                //clip = test4;
                break;
        }
        // Debug.Log($"[SoundManager] scene: {curPlace}, clip: {(clip == null ? "null" : clip.name)}");
        if (clip != null)
        {
            PlayBGM(clip);
        }
        else
        {
            Stop(); // 이 부분 활성화
        }
    }

    #region SFX

    public void PlaySFX(AudioClip audioClip, float pitch)//효과음 재생
    {
        if(audioClip == null) return;

        var sfx = soundPool.Get(false);
        sfx.Play(audioClip, pitch);
        StartCoroutine(ReturnAfterPlay(sfx, audioClip.length));
    }

    public void PlaySFXAtPosition(AudioClip audioClip, Vector3 worldPosition, float volume = 1f, float maxDistance = 16f, float pitch = 0f)
    {
        if (audioClip == null) return;

        var sfx = soundPool.Get(false);
        sfx.transform.position = worldPosition;

        if (sfx.audioSource != null)
        {
            sfx.audioSource.spatialBlend = 1f;
            sfx.audioSource.maxDistance = maxDistance;
            sfx.audioSource.minDistance = 5f;
            sfx.audioSource.rolloffMode = AudioRolloffMode.Linear;
            sfx.audioSource.volume = volume;
            sfx.audioSource.dopplerLevel = 0f;
        }
        sfx.Play(audioClip, pitch);

        StartCoroutine(ReturnAfterPlay(sfx, audioClip.length));
    }

    #endregion

    #region Call by Clip Name

    public AudioClip GetClipByName(string clipName)
    {
        // 프로젝트별로 AudioClip 관리 방식에 따라 다르게 구현
        // 예시: Resources.Load, Dictionary, 배열 등에서 찾아서 반환
        return Resources.Load<AudioClip>($"Sounds/SFX/{clipName}");
    }

    public void PlaySFXAtPositionByClipName(string clipName, Vector3 worldPosition, float volume = 1f, float maxDistance = 16f, float pitch = 0f)
    {
        var clip = GetClipByName(clipName);
        PlaySFXAtPosition(clip, worldPosition, volume, maxDistance, pitch);
    }

    #endregion

    #region BGM
    public void PlayBGM(AudioClip audioClip)
    {

        if (bgmSource == null)
        {
            Debug.LogWarning("[SoundManager] bgmSource is null!");
            return;
        }

        if (audioClip == null)
        {
            Debug.Log("[SoundManager] audioClip is null, stopping BGM.");
            Stop();
            return;
        }

        if (bgmSource.clip == audioClip && bgmSource.isPlaying)
            return;

        Debug.Log($"[SoundManager] Playing new BGM: {audioClip.name}");

        currentBGMClip = audioClip;

        StopAllCoroutines();
        StartCoroutine(FadeInBGM(audioClip, 1f));
        //if (audioClip == null)
        //{
        //    Stop();
        //    return;
        //}

        //// 같은 BGM이면 재생 안 함
        //if (bgmSource.clip == audioClip && bgmSource.isPlaying)
        //    return;

        //currentBGMClip = audioClip;

        //StopAllCoroutines(); // 페이드 중복 방지
        //StartCoroutine(FadeInBGM(audioClip, 1f)); // 1초간 페이드 인
    }

    //private IEnumerator FadeInBGM(AudioClip clip, float duration)//fade = bgm 천천히 바뀌게 하기
    //{
    //    if (bgmSource.isPlaying)
    //        yield return StartCoroutine(FadeOutBGM(duration * 0.5f));

    //    bgmSource.clip = clip;
    //    bgmSource.volume = 0f;
    //    bgmSource.Play();

    //    while (bgmSource.volume < bgmVolume)
    //    {
    //        bgmSource.volume += Time.deltaTime / duration;
    //        yield return null;
    //    }

    //    bgmSource.volume = bgmVolume;
    //}

    //private IEnumerator FadeOutBGM(float duration)
    //{
    //    float startVolume = bgmSource.volume;

    //    while (bgmSource.volume > 0)
    //    {
    //        bgmSource.volume -= startVolume * Time.deltaTime / duration;
    //        yield return null;
    //    }

    //    bgmSource.Stop();
    //    bgmSource.clip = null;
    //    bgmSource.volume = startVolume;
    //}

    private IEnumerator FadeInBGM(AudioClip clip, float duration)
    {
        if (bgmSource.isPlaying)
            yield return StartCoroutine(FadeOutBGM(duration * 0.5f));

        bgmSource.clip = clip;
        bgmSource.volume = 1f; // mixer가 제어
        bgmSource.Play();

        yield return null;
    }

    private IEnumerator FadeOutBGM(float duration)
    {
        bgmSource.Stop();
        yield return null;
    }
    #endregion

    private void Stop()
    {
        // Debug.Log("[SoundManager] Stop called");

        if (bgmSource != null)
        {
            // Debug.Log("[SoundManager] Stopping BGM");
            StopAllCoroutines(); 
            StartCoroutine(FadeOutBGM(3f)); 
        }
        else
        {
            // Debug.LogWarning("[SoundManager] bgmSource is NULL");
        }
    }
    public IEnumerator ReturnAfterPlay(SoundSource source, float duration)//사운드 재생 끝나면 풀로 돌아가기
    {
        yield return new WaitForSeconds(duration);
        soundPool.Return(source);
    }

    //public void SetSfxVolume(float volume)//볼륨 바뀌면 바로 적용
    //{
    //    sfxVolume = volume;

    //    foreach (var source in soundPool.ActiveSources)
    //        source.SetVolume(sfxVolume);
    //    //soundSource?.SetVolume(sfxVolume);
    //}

    //public void SetBgmVolume(float volume)
    //{
    //    bgmVolume = volume;

    //    if (bgmSource != null)
    //        bgmSource.volume = bgmVolume;
    //}

    #region Volume Settings
    public void SetMasterVolume(float volume)
    {
        masterVolume = volume;
        float dB = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20f;
        audioMixer.SetFloat(MASTER_VOLUME_PARAM, dB);
    }

    public void SetSfxVolume(float volume)
    {
        sfxVolume = volume;
        float dB = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20f;
        audioMixer.SetFloat(SFX_VOLUME_PARAM, dB);
    }

    public void SetBgmVolume(float volume)
    {
        bgmVolume = volume;
        float dB = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20f;
        audioMixer.SetFloat(BGM_VOLUME_PARAM, dB);
    }
    #endregion

    #region Save & Load & Default Setting

    public void SaveVolumeSettings()
    {
        PlayerPrefs.SetFloat("MasterVolume", masterVolume);
        PlayerPrefs.SetFloat("BgmVolume", bgmVolume);
        PlayerPrefs.SetFloat("SfxVolume", sfxVolume);
        PlayerPrefs.Save();
    }

    public void LoadVolumeSettings()
    {
        masterVolume = PlayerPrefs.GetFloat("MasterVolume", 0.5f); // 기본값 0.5
        bgmVolume = PlayerPrefs.GetFloat("BgmVolume", 1.0f);
        sfxVolume = PlayerPrefs.GetFloat("SfxVolume", 1.0f);

        SetMasterVolume(masterVolume);
        SetBgmVolume(bgmVolume);
        SetSfxVolume(sfxVolume);
    }

    public void ResetToDefaultVolume()
    {
        SetMasterVolume(DEFAULT_MASTER_VOLUME);
        SetBgmVolume(DEFAULT_BGM_VOLUME);
        SetSfxVolume(DEFAULT_SFX_VOLUME);

        SaveVolumeSettings();
    }

    #endregion

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Debug.Log($"[SoundManager] OnSceneLoaded → {scene.name}");
        PlayCurPlaceBGM(scene.name);
    }
}


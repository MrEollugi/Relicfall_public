using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using static Unity.VisualScripting.Member;

public class SoundSource : MonoBehaviour , SoundInterface//풀에서 만들어서 사용되는 사운드
{
    public AudioSource audioSource;
    public AudioMixerGroup outputMixerGroup;
    //public float currentVolume;
    public bool IsPlaying => audioSource != null && audioSource.isPlaying;

    public void Init(bool loop)
    {
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.loop = loop;
        audioSource.volume = 1f;
        audioSource.outputAudioMixerGroup = outputMixerGroup;
    }

    public void Play(AudioClip clip, float pitchVar = 0)
    {
        if (clip == null) return;

        audioSource.clip = clip;
        audioSource.pitch = 1f + Random.Range(-pitchVar, pitchVar);
        audioSource.volume = 1f;
        audioSource.Play();
    }

    public void Stop()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
            audioSource.clip = null;
        }
    }

    public void SetVolume(float volume)
    {
        
    }

    //public void Init(bool loop, float volume)
    //{
    //    if (audioSource == null)
    //    {
    //        audioSource = gameObject.AddComponent<AudioSource>();
    //    }

    //    audioSource.loop = loop;
    //    currentVolume = volume;
    //    audioSource.volume = volume;
    //}
    //public void Play(AudioClip audioClip, float pitch/*, float volume*/)
    //{
    //    if (audioClip == null) return;
    //    audioSource.clip = audioClip;
    //    audioSource.pitch = 1f + Random.Range(-pitch, pitch);
    //    audioSource.volume = currentVolume;
    //    //audioSource.volume = volume;      // 여기에서 실시간 볼륨 직접 적용!
    //    //currentVolume = volume;
    //    audioSource.Play();
    //}

    //public void Stop()
    //{
    //    if (audioSource != null)
    //    {
    //        audioSource.Stop();
    //        audioSource.clip = null;
    //    }
    //}

    //public void SetVolume(float volume)
    //{
    //    currentVolume = volume;
    //    if (audioSource != null) audioSource.volume = volume;
    //}
}

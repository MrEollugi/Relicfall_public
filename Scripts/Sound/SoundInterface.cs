using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface SoundInterface //사운드 인터페이스
{
    void Play(AudioClip clip, float pitchVar = 0f/*, float volume = 0.5f*/);//clip은 재생할 파일, pitchVar는 재생할 파일의 높낮이를 +- 0.n f만큼 만들어서 반복되는 효과음을 다양하게 보이게함
    
    void Stop();

    void SetVolume(float volume);
}

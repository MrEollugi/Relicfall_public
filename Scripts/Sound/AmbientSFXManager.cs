using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientSFXManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] ambientClips;
    private float ambientTimer, nextAmbientTime;

    void Start() { SetNextTime(); }
    void Update()
    {
        ambientTimer += Time.deltaTime;
        if (ambientTimer > nextAmbientTime)
        {
            PlayRandomAmbient();
            ambientTimer = 0f;
            SetNextTime();
        }
    }
    void SetNextTime() => nextAmbientTime = Random.Range(20f, 60f);

    void PlayRandomAmbient()
    {
        if (ambientClips == null || ambientClips.Length == 0) return;
        var clip = ambientClips[Random.Range(0, ambientClips.Length)];
        SoundManager.Instance.PlaySFX(clip, 1.0f);
    }
}

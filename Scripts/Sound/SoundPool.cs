using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundPool//SoundSource를 큐에 저장해두고 사용
{
    private readonly GameObject prefab;
    private readonly Transform parent;
    private readonly Queue<SoundSource> pool = new();
    private readonly List<SoundSource> active = new List<SoundSource>();

    private readonly AudioMixerGroup mixerGroup;

    public SoundPool(GameObject prefab, Transform parent, AudioMixerGroup mixerGroup)
    {
        this.prefab = prefab;
        this.parent = parent;
        this.mixerGroup = mixerGroup;
    }

    public IEnumerable<SoundSource> ActiveSources => active; 

    //public SoundSource Get(bool loop, float volume)
    //{
    //    SoundSource source = null;

    //    while (pool.Count > 0)
    //    {
    //        var candidate = pool.Dequeue();
    //        if (!candidate.IsPlaying)
    //        {
    //            source = candidate;
    //            break;
    //        }
    //    }

    //    if (source == null)
    //    {
    //        foreach (var s in active)
    //        {
    //            if (!s.IsPlaying)
    //            {
    //                source = s;
    //                break;
    //            }
    //        }
    //    }

    //    if (source == null)
    //        source = CreateNew();

    //    source.Init(loop, volume);
    //    source.gameObject.SetActive(true);

    //    if (!active.Contains(source))
    //        active.Add(source);

    //    return source;
    //}

    public SoundSource Get(bool loop)
    {
        SoundSource source = null;

        while (pool.Count > 0)
        {
            var candidate = pool.Dequeue();
            if (!candidate.IsPlaying)
            {
                source = candidate;
                break;
            }
        }

        if (source == null)
        {
            foreach (var s in active)
            {
                if (!s.IsPlaying)
                {
                    source = s;
                    break;
                }
            }
        }

        if (source == null)
            source = CreateNew();

        source.outputMixerGroup = mixerGroup;
        source.Init(loop);
        source.gameObject.SetActive(true);

        if (!active.Contains(source))
            active.Add(source);

        return source;
    }

    public void Return(SoundSource source)
    {
        source.Stop();
        source.gameObject.SetActive(false);

        active.Remove(source);
        pool.Enqueue(source);
    }

    private SoundSource CreateNew()
    {
        GameObject go = Object.Instantiate(prefab, parent);
        return go.GetComponent<SoundSource>();
    }
}

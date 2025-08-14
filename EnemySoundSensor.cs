using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySoundSensor : MonoBehaviour
{
    private EnemyController enemy;
    private SoundEvent currentSound;

    [Header("Hearing Settings")]
    public float hearingMultiplier = 1f;

    private void Awake()
    {
        enemy = GetComponent<EnemyController>();
    }

    public void OnSoundHeard(SoundEvent sound)
    {
        float distance = Vector3.Distance(transform.position, sound.source);
        float effectiveRadius = sound.radius * hearingMultiplier;

        if (distance > effectiveRadius)
            return;

        if (currentSound == null || sound.priority > currentSound.priority)
        {
            currentSound = sound;
            // Debug.Log($"{gameObject.name} prioritizes sound at {sound.source} (priority {sound.priority})");

        }

        if (enemy.TryGetComponent<Blackboard>(out var bb))
        {
            bb.lastHeardPosition = sound.source;
        }
    }
}

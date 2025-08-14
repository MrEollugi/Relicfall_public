using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEmitter : MonoBehaviour
{
    private float runSoundRadius = 20f;
    private float moveSoundRadius = 12.5f;
    private float attackSoundRadius = 20f;
    public LayerMask enemyLayer;

    public void EmitSound(float radius, float priority)
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, radius, enemyLayer);
        foreach (var hit in hits)
        {
            var sensor = hit.GetComponent<EnemySoundSensor>();
            if (sensor != null)
            {
                var soundEvent = new SoundEvent
                {
                    source = transform.position,
                    radius = radius,
                    priority = priority
                };

                sensor.OnSoundHeard(soundEvent);
            }
        }
    }

    public void EmitMoveSound()
    {
        EmitSound(moveSoundRadius, 1);
    }
    public void EmitRunSound()
    {
        EmitSound(runSoundRadius, 2);
    }
    public void EmitAttackSound()
    {
        EmitSound(attackSoundRadius, 3);
    }
}

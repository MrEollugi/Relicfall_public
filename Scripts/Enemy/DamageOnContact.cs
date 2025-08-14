using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOnContact : MonoBehaviour
{
    public float damage = 10f;
    public float cooldown = 0.5f;
    private float lastContactTime = -999f;

    void OnTriggerStay(Collider other)
    {
        var enemy = GetComponent<EnemyController>();
        if (enemy != null && !enemy.IsAlive)
            return;

        var player = other.GetComponent<PlayerController>();
        if (player != null && Time.time > lastContactTime + cooldown)
        {
            lastContactTime = Time.time;

            Vector3 attackDirection = (transform.position - player.transform.position).normalized;
            player.TakeDamage(damage, attackDirection);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SpikeTrap : MonoBehaviour, IDungeonGimmick
{
    private float damage = 10f;

    void Start()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    public void Initialize(DungeonThemeData themeData)
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<PlayerController>();

        if (player != null)
        {
            player.TakeDamage(damage);
            // 넉백?
        }
    }

    // IDungeonGimmick.OnTriggerEnter 명세 및 매칭
    void IDungeonGimmick.OnTriggerEnter(PlayerController player) => OnTriggerEnter(player.GetComponent<Collider>());
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorCollider : MonoBehaviour
{
    private void Awake()
    {
        var colliders = GetComponents<BoxCollider>();

        foreach (var c in colliders)
        {
            c.isTrigger = true;
        }

        var rb = gameObject.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 13)
        {
            other.gameObject.SetActive(false);
        }
    }
}

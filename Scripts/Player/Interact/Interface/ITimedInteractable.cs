using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITimedInteractable
{
    float InteractionTime { get; }
    void OnInteractionComplete(PlayerController player);
    void OnInteractionCanceled();
}

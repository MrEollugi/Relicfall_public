using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCInteractor : MonoBehaviour, IImmediateInteractable
{
    //[SerializeField] private DialogueData dialogue;

    public void Interact(PlayerController player)
    {
        //DialogueManager.Instance.StartDialogue(dialogue);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionInteractable : MonoBehaviour, ITimedInteractable, IWorldTooltipDataProvider
{

    [SerializeField] private string targetSceneName;

    private float interactionTime = 2f;
    public float InteractionTime => interactionTime;

    public void OnInteractionComplete(PlayerController player)
    {
        GameManager.Instance.SaveCurrentState();
        // show Loading screnn 
        //SceneManager.LoadScene(targetSceneName);
        GameManager.Instance._loadingManager.StartLoad(targetSceneName);
    }

    public void OnInteractionCanceled()
    {
    }

    public TooltipType GetTooltipType()
    {
        return TooltipType.Wagon;
    }

    public void InitializeTooltip(GameObject tooltipInstance)
    {
    }
}

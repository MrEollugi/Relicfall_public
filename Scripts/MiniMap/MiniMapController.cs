using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapController : Singleton<MiniMapController>
{
    [SerializeField] private Camera miniMapCamera;

    [SerializeField] private GameObject miniMapUI;

    [SerializeField] private float cameraHeight = 50f;

    private bool isOn = false;

    void Awake()
    {
        base.Awake();
    }

    public void Toggle()
    {
        isOn = !isOn;
        miniMapUI.SetActive(isOn);
        miniMapCamera.enabled = isOn;
    }

    public void SetCenter(Vector3 worldPos)
    {
        if (!isOn)
            return;

        miniMapCamera.transform.position = new Vector3(worldPos.x, cameraHeight, worldPos.z);
    }
}

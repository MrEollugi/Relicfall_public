using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LightState
{
    On, Off, Destroyed
}

public class LightSource : MonoBehaviour
{
    public LightState state = LightState.On;
    // private Light light; 진짜 빛이 아닌 마스크 관리
    public float range = 5f;
    public int segments = 20;

    void Start()
    {
        if (state == LightState.On)
            PlayerSightController.Instance.RegisterLightSource(this);
    }

    void OnDestroy()
    {
        // 파괴 시 해제
        if (PlayerSightController.HasInstance)
            PlayerSightController.Instance.UnregisterLightSource(this);
    }

    public void SetState(LightState newState)
    {
        if (state == newState)
            return;

        if (state == LightState.On && PlayerSightController.HasInstance)
        {
            PlayerSightController.Instance.UnregisterLightSource(this);
        }

        state = newState;

        switch (state)
        {
            case LightState.On:
                PlayerSightController.Instance.RegisterLightSource(this);
                break;

            case LightState.Off:
                PlayerSightController.Instance.UnregisterLightSource(this);
                break;

            case LightState.Destroyed:
                Destroy(gameObject);
                break;
        }

        PlayerSightController.Instance.UpdateSight();
    }
}

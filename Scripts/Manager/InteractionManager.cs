using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    public float interactRange = 5f;
    public PlayerController player;
    public TimedInteractionUI interactionUI;

    private ITimedInteractable currentTarget;
    private Coroutine interactionRoutine;
    private InputManager inputManager;

    void Awake()
    {
    }

    void Update()
    {
        // 멀티일 때 내 로컬 유저만 실행
        //if (!IsLocalPlayer()) return;

        inputManager = player.InputManager;

        inputManager.UpdateInputData();
        InputData input = inputManager.GetInput();

        ITimedInteractable hovered = GetHoveredInteractable(out RaycastHit hit);
        bool mouseOnTarget = hovered != null;

        // 1) Start Interact first input isInteractDown
        if (mouseOnTarget && input.isInteractDown)
        {
            StartInteraction(hovered, hit);
        }

        // 2) Cancel condition: leave mouse or release interact key (isInteract == false)
        if (interactionRoutine != null &&
            (!mouseOnTarget || !input.isInteract))
        {
            CancelInteraction();
        }
    }

    ITimedInteractable GetHoveredInteractable(out RaycastHit hit)
    {
        Vector2 mouseScreenPos = inputManager.GetMouseScreenPosition();
        Ray ray = Camera.main.ScreenPointToRay(mouseScreenPos);

        if (Physics.Raycast(ray, out hit))
        {
            float distToPlayer = Vector3.Distance(player.transform.position, hit.point);

            if (distToPlayer > interactRange)
            {
                return null;
            }

            return hit.collider.GetComponent<ITimedInteractable>();
        }
        hit = default;
        return null;
    }

    void StartInteraction(ITimedInteractable target, RaycastHit hit)
    {
        CancelInteraction();

        currentTarget = target;
        interactionRoutine = StartCoroutine(InteractionCoroutine(target, player));
        interactionUI.Show(target.InteractionTime, Input.mousePosition, TimedInteractionUIPositionType.Mouse);
    }

    System.Collections.IEnumerator InteractionCoroutine(ITimedInteractable target, PlayerController player)
    {
        float time = 0f, total = target.InteractionTime;
        while (time < total)
        {
            interactionUI.SetProgress(time / total);
            time += Time.deltaTime;
            yield return null;
        }
        interactionUI.Hide();
        interactionRoutine = null;
        currentTarget = null;

        target.OnInteractionComplete(player);
    }

    void CancelInteraction()
    {
        if (interactionRoutine != null)
        {
            StopCoroutine(interactionRoutine);
            interactionUI.Hide();
            currentTarget?.OnInteractionCanceled();
            interactionRoutine = null;
            currentTarget = null;
        }
    }

    bool IsMouseStillOnTarget(ITimedInteractable target)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, interactRange))
        {
            return hit.collider.GetComponent<ITimedInteractable>() == target;
        }
        return false;
    }

    // 네트워크 라이브러리별로 구현 (예: PhotonView.RPC, Netcode의 ServerRpc 등)
    void SendInteractionRequestToServer(ITimedInteractable target)
    {
        // 예시 (Photon):
        // photonView.RPC("OnRequestInteraction", RpcTarget.MasterClient, targetId);

        // 예시 (Netcode for GameObjects):
        // RequestInteractionServerRpc(target.NetworkObjectId);

        Debug.Log("상호작용 완료 요청을 서버(호스트)에게 보냄!");
    }

    // 서버에서 상호작용 승인/실패 결과를 받을 때 호출되는 함수 (예시)
    public void OnInteractionApproved(/* 상호작용 오브젝트 정보 등 */)
    {
        // 내 PlayerController에 적용
        // ex) currentTarget.OnInteractionComplete(player);
        // 모든 클라이언트에서 오브젝트 오픈, 아이템 지급 등
        Debug.Log("상호작용 승인됨! 효과 적용");
    }

    bool IsLocalPlayer()
    {
        // 네트워크 프레임워크에 따라 구현
        // 예: PhotonView.IsMine, NetworkObject.IsLocalPlayer 등
        return true; // 싱글플레이 테스트용
    }
}

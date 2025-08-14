using Unity.VisualScripting;
using UnityEngine;

public enum TooltipType
{
    Item,
    NPC,
    Wagon,
    Chest
}
public interface IWorldTooltipDataProvider
{
    TooltipType GetTooltipType();
    void InitializeTooltip(GameObject tooltipInstance);
}

[RequireComponent(typeof(Collider))]
public class WorldTooltipTarget : MonoBehaviour
{
    private IWorldTooltipDataProvider dataProvider;
    private GameObject tooltipInstance;

    private PlayerController player;

    private bool isMouseOver = false;

    private void Awake()
    {
        dataProvider = GetComponent<IWorldTooltipDataProvider>();
    }

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            player = GameManager.Instance.CurrentPlayer;
        }
    }

    private void Update()
    {
        if (!isMouseOver || dataProvider == null) return;

        if (player == null) return;

        float distance = Vector3.Distance(player.transform.position, transform.position);

        // 거리 안에 있고 툴팁이 없으면 생성
        if (distance <= player.InteractRange && tooltipInstance == null)
        {
            TooltipType type = dataProvider.GetTooltipType();
            tooltipInstance = InGameUIManager.Instance.ShowWorldTooltip(transform.position, type, dataProvider);
        }
        // 거리 벗어났으면 툴팁 제거
        else if (distance > player.InteractRange && tooltipInstance != null)
        {
            ClearTooltip();
        }
    }

    private void OnMouseEnter()
    {
        isMouseOver = true;
    }

    private void OnMouseExit()
    {
        isMouseOver = false;
        ClearTooltip();
    }

    private void OnDestroy()
    {
        ClearTooltip();
    }

    private void ClearTooltip()
    {
        if (tooltipInstance != null)
        {
            Destroy(tooltipInstance);
            tooltipInstance = null;
        }
    }
}

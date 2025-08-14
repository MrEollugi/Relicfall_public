using UnityEngine;

public class WorldTooltipUI : MonoBehaviour
{
    [SerializeField] protected Vector3 positionOffset = new Vector3(0f, 1.5f, 0f);

    public virtual void Show()
    {
        gameObject.SetActive(true);
    }

    public virtual void Hide()
    {
        gameObject.SetActive(false);
    }

    public virtual void SetPosition(Vector3 worldPosition)
    {
        transform.position = worldPosition + positionOffset;
        transform.forward = Camera.main.transform.forward;
    }
}

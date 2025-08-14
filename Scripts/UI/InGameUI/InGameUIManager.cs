using UnityEngine;

public class InGameUIManager : MonoBehaviour
{
    public static InGameUIManager Instance { get; private set; }

    [SerializeField] private GameObject damageTextPrefab;

    private Canvas canvas;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        canvas = GetComponent<Canvas>();
    }

    public void ShowDamageText(Vector3 worldPosition, float damageAmount, bool isCrit = false)
    {
        Vector3 spawnPos = worldPosition + Vector3.up * 1.5f;

        Vector3 offset = new Vector3(
            Random.Range(-0.5f, 0.5f),
            Random.Range(-0.5f, 0.5f),
            0f
        );

        spawnPos += offset;
        spawnPos += new Vector3(0f, 0f, 1.2f);

        GameObject go = Instantiate(
            damageTextPrefab,
            spawnPos,
            Quaternion.Euler(45f, 0f, 0f),
            canvas.transform
        );

        var textUI = go.GetComponent<DamageTextUI>();
        textUI.SetDamage(damageAmount, isCrit);
    }

    public GameObject ShowWorldTooltip(Vector3 worldPosition, TooltipType type, IWorldTooltipDataProvider dataProvider)
    {
        GameObject prefab = TooltipPrefabRegistry.Instance?.GetTooltipPrefab(type);
        if (prefab == null) return null;

        GameObject tooltipInstance = Instantiate(prefab, canvas.transform);

        if (tooltipInstance.TryGetComponent<WorldTooltipUI>(out var tooltipUI))
        {
            tooltipUI.SetPosition(worldPosition);
            tooltipUI.Show();
        }

        dataProvider.InitializeTooltip(tooltipInstance);

        return tooltipInstance;
    }
}

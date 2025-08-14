using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    private static InventoryManager instance;

    [Header("UI Root (Canvas)")]
    public Transform uiRoot;

    public static InventoryManager Instance 
    { 
        get 
        { 
            if (instance == null)
                instance = new GameObject("InventoryManager").AddComponent<InventoryManager>();

            return instance; 
        } 
    }

    private InventoryUI inventoryUI;
    public InventoryUI InventoryUI { get => inventoryUI; set => inventoryUI = value; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}

using UnityEngine;
using UnityEngine.UI;

public class JobButtonUI : MonoBehaviour
{
    [SerializeField] private InventoryUI inventoryUI;
    [SerializeField] private ItemData item;

    [SerializeField] private Button porterButton;
    [SerializeField] private Button warriorButton;
    [SerializeField] private Button wizardButton;

    private InventoryGrid inventory;

    void Start()
    {
        // 버튼이 할당되었는지 확인하고 클릭 이벤트 리스너 추가
        if (porterButton != null)
        {
            porterButton.onClick.AddListener(SetPorter);
        }
        if (warriorButton != null)
        {
            warriorButton.onClick.AddListener(SetWarrior);
        }
        if (wizardButton != null)
        {
            wizardButton.onClick.AddListener(SetWizard);
        }
    }

    private void SetPorter()
    {
        SetInventory(6, 1);
    }

    private void SetWarrior()
    {
        SetInventory(6, 1);
    }

    private void SetWizard()
    {
        SetInventory(6, 1);
    }

    private void SetInventory(int w, int h)
    {
        int width = w;
        int height = h;
        inventory = new InventoryGrid(width, height);

        // 정해진 크기만큼 슬롯 생성
        inventoryUI.Init(width, height, inventory);
    }
}

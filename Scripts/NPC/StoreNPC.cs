using UnityEngine;

public class StoreNPC : BaseNPC
{
    // [SerializeField] private 상점데이터 data;
    [SerializeField] private int dialogDataIndex;
    [SerializeField] private Store store;
    private PlayerController playerController;

    public override void Interact(PlayerController player)
    {
        base.Interact(player);
        playerController = player;

        DialogSystem.Instance.StartDialog(1, Name, this, UIPanelID.Store);
    }

    public override void DialogFinished()
    {
        base.DialogFinished();
    }

    public override void Act()
    {
        sell();
    }

    private void sell()
    {
        store.SellItem();
    }
}

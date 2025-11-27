using UnityEngine;
using SF = UnityEngine.SerializeField;

public class Stove : WorkTable
{
    private void OnTriggerEnter(Collider other)
    {
        if (placedItem is not null) return;
        if (!CheckTriggeredItem(other, out var item)) return;
        
        PlaceItem(item);
    }

    public override bool Interact(PlayerController player)
    {
        return base.Interact(player);
    }

    public override void PlaceItem(Item item)
    {
        if (item.IsDone())
        {
            item.ActivatePhysics();
            return;
        }
        base.PlaceItem(item);
        IsWorking = true;
        ActivateUI();
    }

    public override Item DisplaceItem()
    {
        DeactivateUI();
        return base.DisplaceItem();
    }
}

using UnityEngine;
using SF = UnityEngine.SerializeField;

public class Box : PlaceTable
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
        base.PlaceItem(item);
    }

    public override Item DisplaceItem()
    {
        return base.DisplaceItem();
    }
}
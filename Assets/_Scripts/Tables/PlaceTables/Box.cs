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

    public override bool Interact(PlayerController_Stage player)
    {
        if (placedItem is not Plate plate) return base.Interact(player);
        if (player.pickedItem is null)
        {
            player.AttachItem(DisplaceItem());
            return true;
        }
        if (!plate.IsAbleToStack(player.pickedItem)) return false;
        plate.StackIngredient(player.DetachItem());
        return true;
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
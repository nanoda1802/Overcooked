using UnityEngine;

public class Countertop : PlaceTable
{
    private void OnTriggerEnter(Collider other)
    {
        if (!CheckTriggeredItem(other, out var item)) return;
        PlaceItem(item);
    }

    public override void PlaceItem(Item item)
    {
        switch (placedItem)
        {
            case Plate plate when item is not Plate:
                plate.StackIngredient(item);
                break;
            case null when item is Plate && item.IsDone():
                base.PlaceItem(item);
                break;
            default:
                item.ActivatePhysics();
                break;
        }
    }

    public override Item DisplaceItem()
    {
        return base.DisplaceItem();
    }
}

using UnityEngine;

public class Countertop : PlaceTable
{
    private void OnTriggerEnter(Collider other)
    {
        if (!CheckTriggeredItem(other, out var item)) return;
        PlaceItem(item);
    }

    protected override void PlaceItem(Item item)
    {
        if (placedItem is Plate plate && item is not Plate)
        {
            plate.StackIngredient(item);
        }
        else if (placedItem is null && item is Plate && item.IsDone())
        {
            base.PlaceItem(item);
        }
        else
        {
            item.ActivatePhysics();
        }
    }
}

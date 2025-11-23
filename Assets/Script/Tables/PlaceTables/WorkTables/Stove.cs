using UnityEngine;
using SF = UnityEngine.SerializeField;

public class Stove : WorkTable
{
    private void OnTriggerEnter(Collider other)
    {
        if (placedItem is not null || !CheckTriggeredItem(other, out var item)) return;
        PlaceItem(item);
    }
    
    protected override void PlaceItem(Item item)
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

    protected override Item DisplaceItem()
    {
        DeactivateUI();
        return base.DisplaceItem();
    }
}

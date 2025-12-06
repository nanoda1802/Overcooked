using System.Collections.Generic;
using UnityEngine;
using SF = UnityEngine.SerializeField;

public class DishRack : PlaceTable
{
    [SF] private float offsetY;
    private readonly Stack<Plate> _plates = new(10);

    public override bool Interact(PlayerController player)
    {
        if (player.pickedItem is not null) return false;
        if (_plates.Count == 0) return false;
        
        player.AttachItem(DisplaceItem());
        return true;
    }

    public override void PlaceItem(Item item)
    {
        if (item is not Plate plate) return;
        
        plate.IsPlaced = true;
        
        plate.SetParent(pivot);
        SetLocalPos(plate);
        
        _plates.Push(plate);
        plate.IsInDishRack = true;
    }

    public override Item DisplaceItem()
    {
        Plate plate = _plates.Pop();
        plate.IsInDishRack = plate.IsPlaced = false;
        plate.RemoveParent();
        return plate;
    }

    private void SetLocalPos(Plate plate)
    {
        Vector3 localPos = _plates.TryPeek(out Plate peekPlate) ? peekPlate.transform.localPosition : Vector3.zero;
        localPos.y += offsetY;
        plate.transform.localPosition = localPos;
    }
}

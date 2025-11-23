using System.Collections.Generic;
using UnityEngine;
using SF = UnityEngine.SerializeField;

public class DishRack : PlaceTable
{
    [SF] private float offsetY;
    private readonly Stack<Plate> _plates = new(10);

    public override bool Interact(PlayerController player)
    {
        if (player.pickedItem is not null || _plates.Count == 0) return false;
        player.AttachItem(_plates.Pop());
        return true;
    }

    public void PutOnPlate(Item plate) // [임시]
    {
        PlaceItem(plate);
    }

    protected override void PlaceItem(Item item)
    {
        if (item is not Plate plate) return;
        plate.SetParent(pivot);
        plate.IsPlaced = true;
        Vector3 localPos = _plates.TryPeek(out Plate peekPlate) ? peekPlate.transform.localPosition : Vector3.zero;
        localPos.y += offsetY;
        plate.transform.localPosition = localPos;
        _plates.Push(plate);
    }
}

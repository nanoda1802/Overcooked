using UnityEngine;
using SF = UnityEngine.SerializeField;

public class Box : PlaceTable
{
    private void OnTriggerEnter(Collider other)
    {
        if (placedItem is not null || !CheckTriggeredItem(other, out var item)) return;
        PlaceItem(item);
    }
}
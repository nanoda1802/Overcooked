using UnityEngine;
using SF = UnityEngine.SerializeField;

public class Bin : Table
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Item")) return;
        if (!other.TryGetComponent(out Item item)) return;
        if (item is Plate) return;
        if (!item.IsThrown && !item.IsFalling) return;
        
        item.Deactivate();
    }
    
    public override bool Interact(PlayerController player)
    {
        if (player.pickedItem is null) return false;
        
        if (player.pickedItem is Plate plate) plate.ClearPlate();
        else player.DetachItem().Deactivate();
        
        return true;
    }
}

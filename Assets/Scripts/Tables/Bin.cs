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
    
    public override bool Interact(InStagePlayerController inStagePlayer)
    {
        if (inStagePlayer.pickedItem is null) return false;
        
        if (inStagePlayer.pickedItem is Plate plate) plate.ClearPlate();
        else inStagePlayer.DetachItem().Deactivate();
        
        return true;
    }
}

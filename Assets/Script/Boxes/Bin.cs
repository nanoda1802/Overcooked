using UnityEngine;
using SF = UnityEngine.SerializeField;

public class Bin : MonoBehaviour, IInteractable
{
    protected void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Item")) return;
        
        if (other.TryGetComponent(out Item item) && (item.IsThrown || item.IsFalling) && item.type != ItemType.Plate)
        {
            item.DisposeItem();
        }
    }
    
    public bool Interact(PlayerController player)
    {
        if (player.pickedItem is null) return false;
        if (player.pickedItem is Plate plate)
        {
            plate.ClearPlate();
            return true;
        } 
        Item item = player.pickedItem; // Detach에서 참조를 끊어서 필요!
        player.DetachItem();
        item.DisposeItem();
        return true;
    }
    
    public bool BeginWork(PlayerController player) { return false; }
    public void StopWork(PlayerController player) {}
}

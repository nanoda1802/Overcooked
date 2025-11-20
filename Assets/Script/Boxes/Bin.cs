using UnityEngine;
using SF = UnityEngine.SerializeField;

public class Bin : MonoBehaviour, IInteractable
{
    protected void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Item")) return;
        
        if (other.TryGetComponent(out Item item) && (item.IsThrown || item.IsFalling))
        {
            item.StopThrowing();
            Destroy(item.gameObject); // 임시... 추후 Pool로
        }
    }
    
    public void Interact(PlayerController player)
    {
        if (player.pickedItem is null) return;
        GameObject itemObj = player.pickedItem.gameObject;
        player.DetachItem();
        Destroy(itemObj); // 임시... 추후 Pool로
    }
    
    public bool BeginWork(PlayerController player) { return false; }
    public void StopWork(PlayerController player) {}
}

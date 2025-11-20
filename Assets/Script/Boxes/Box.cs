using System.Linq;
using UnityEngine;
using SF = UnityEngine.SerializeField;

public interface IInteractable
{
    public PlayerController Player { get; set; }
    public void Interact();
}

public class Box : MonoBehaviour, IInteractable
{
    public PlayerController Player { get; set; }
    public Item placedItem;
    [SF] protected Transform pivot;
    [SF] protected ItemType[] availableItems;

    protected void OnTriggerEnter(Collider other)
    {
        if (placedItem is not null) return;

        if (other.TryGetComponent(out Item item) 
            && (item.IsThrown || item.IsFalling) 
            && availableItems.Contains(item.type))
        {
            AttachItem(item);
        }
    }

    public virtual void Interact()
    {
        if (Player is null) return;

        if (Player.pickedItem is null && placedItem is not null)
        {
            Item item = placedItem; // detach에서 참조를 끊어서 필요한 변수
            DetachItem();
            Player.AttachItem(item);
        }
        else if (Player.pickedItem is not null && placedItem is null && availableItems.Contains(Player.pickedItem.type))
        {
            Item item = Player.pickedItem; // detach에서 참조를 끊어서 필요한 변수
            Player.DetachItem();
            AttachItem(item);
        }
    }

    protected virtual void AttachItem(Item item)
    {
        item.SetParent(pivot);
        placedItem = item;
    }

    protected virtual void DetachItem()
    {
        placedItem.RemoveParent();
        placedItem = null;
    }
}
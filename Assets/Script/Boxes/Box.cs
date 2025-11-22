using System.Linq;
using UnityEngine;
using SF = UnityEngine.SerializeField;

public interface IInteractable
{ 
    public bool Interact(PlayerController player);
    public bool BeginWork(PlayerController player);
    public void StopWork(PlayerController player);
}

public class Box : MonoBehaviour, IInteractable
{
    [SF] protected Item placedItem;
    [SF] protected Transform pivot;
    [SF,Range(0f,1f)] private float scaleOffset;
    [SF] protected ItemType[] availableItems;
    protected bool IsWorking;
    
    [SF] protected Canvas canvas;
    
    protected void OnTriggerEnter(Collider other)
    {
        if (placedItem is not null || !other.CompareTag("Item")) return;
        
        if (other.TryGetComponent(out Item item) 
            && (item.IsThrown || item.IsFalling) 
            && availableItems.Contains(item.type))
        {
            AttachItem(item);
        }
    }

    public virtual bool Interact(PlayerController player)
    {
        if (player.pickedItem is null && placedItem is not null)
        {
            Item item = placedItem; // detach에서 참조를 끊어서 필요한 변수
            DetachItem();
            player.AttachItem(item);
            return true;
        }
        
        if (player.pickedItem is not null && placedItem is null && availableItems.Contains(player.pickedItem.type))
        {
            Item item = player.pickedItem; // detach에서 참조를 끊어서 필요한 변수
            player.DetachItem();
            AttachItem(item);
            return true;
        }
        
        return false;
    }

    public virtual bool BeginWork(PlayerController player) { return false; }
    public virtual void StopWork(PlayerController player) { }

    public virtual void AttachItem(Item item)
    {
        item.SetParent(pivot);
        placedItem = item;
        pivot.localScale *= scaleOffset;
    }

    protected virtual void DetachItem()
    {
        pivot.localScale = Vector3.one;
        placedItem.RemoveParent();
        placedItem = null;
    }

    protected virtual void Work()
    {
        IsWorking = !placedItem.IsDone();
    }
    
    protected virtual void ActivateCanvas()
    {
        canvas?.gameObject.SetActive(true);
    }
    
    protected void DeactivateCanvas()
    {
        canvas?.gameObject.SetActive(false);
    }
}
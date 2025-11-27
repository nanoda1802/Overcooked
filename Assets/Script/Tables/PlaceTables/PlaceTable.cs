using System.Linq;
using UnityEngine;
using SF = UnityEngine.SerializeField;

public class PlaceTable : Table
{
    [SF] protected Item placedItem;
    [SF,Range(0f,1f)] private float scaleOffset;
    [SF] protected ItemType[] availableItems;
    
    public override bool Interact(PlayerController player)
    {
        if (player.pickedItem is null && placedItem is not null)
        {
            player.AttachItem(DisplaceItem());
            return true;
        }
        if (player.pickedItem is not null && placedItem is null && availableItems.Contains(player.pickedItem.type))
        {
            PlaceItem(player.DetachItem());
            return true;
        }
        return false;
    }
    
    public virtual void PlaceItem(Item item)
    {
        item.SetParent(pivot);
        placedItem = item;
        item.IsPlaced = true;
        pivot.localScale *= scaleOffset;
    }
    
    public virtual Item DisplaceItem() { 
        pivot.localScale = Vector3.one;
        
        Item item = placedItem;
        placedItem = null;
        
        item.IsPlaced = false;
        item.RemoveParent();
        return item; 
    }
    
    protected bool CheckTriggeredItem(Collider other, out Item checkedItem)
    {
        checkedItem = null;
        
        if (!other.CompareTag("Item")) return false;
        if (!other.TryGetComponent(out Item item)) return false;
        if (!availableItems.Contains(item.type)) return false;
        if (!item.IsThrown && !item.IsFalling) return false;
        
        checkedItem = item;
        return true;
    }
}

using UnityEngine;
using SF = UnityEngine.SerializeField;

public class Box : MonoBehaviour, IInteractable
{
    public PlayerController Player { get; set; }
    public Item placedItem;
    [SF] private Transform pivot;

    private void OnTriggerEnter(Collider other)
    {
        if (placedItem is not null) return;

        if (other.TryGetComponent(out Item item) && (item.isThrown || item.isFalling))
        {
            AttachItem(item);
        }
    }

    public void Interact()
    {
        if (Player is null) return;

        if (Player.pickedItem is null && placedItem is not null)
        {
            Item item = placedItem; // detach에서 참조를 끊어서 필요한 변수
            DetachItem();
            Player.AttachItem(item);
        }
        else if (Player.pickedItem is not null && placedItem is null)
        {
            Item item = Player.pickedItem; // detach에서 참조를 끊어서 필요한 변수
            Player.DetachItem();
            AttachItem(item);
        }
    }

    private void AttachItem(Item item)
    {
        item.rb.isKinematic = true;
        item.col.enabled = false;
        item.transform.SetParent(pivot);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;
        placedItem = item;
    }

    private void DetachItem()
    {
        placedItem.transform.SetParent(null);
        placedItem = null;
    }
}
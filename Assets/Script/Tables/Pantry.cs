using UnityEngine;
using SF = UnityEngine.SerializeField;

public class Pantry : Table
{
    [SF] private ItemType type;
    [SF] private GameObject[] items;

    private void Start()
    {
        pivot.GetChild((int)type).gameObject.SetActive(true);
    }

    public override bool Interact(PlayerController player)
    {
        if (player.pickedItem is not null) return false;

        // 일단 Instantiate로... 추후 pool로 변경
        GameObject itemObj = Instantiate(items[(int)type], pivot.position, Quaternion.identity);
        if (itemObj.TryGetComponent(out Item item)) player.AttachItem(item);
        return true;
    }
}
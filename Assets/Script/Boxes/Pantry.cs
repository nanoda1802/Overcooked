using UnityEngine;
using SF = UnityEngine.SerializeField;

public class Pantry : MonoBehaviour, IInteractable
{
    public PlayerController Player { get; set; }
    [SF] private ItemType type;
    [SF] private Transform pivot;
    [SF] private GameObject[] items;

    private void Awake()
    {
        pivot.GetChild((int)type).gameObject.SetActive(true);
    }

    public void Interact()
    {
        if (Player is null || Player.pickedItem is not null) return;

        // 일단 Instantiate로... 추후 pool로 변경
        GameObject itemObj = Instantiate(items[(int)type], pivot.position, Quaternion.identity);
        if (itemObj.TryGetComponent(out Item item))
        {
            Player.pickedItem = item;
            Player.AttachItem(item);
        }
    }
}
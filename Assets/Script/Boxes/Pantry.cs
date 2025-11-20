using UnityEngine;
using SF = UnityEngine.SerializeField;

public class Pantry : MonoBehaviour, IInteractable
{
    [SF] private ItemType type;
    [SF] private Transform pivot;
    [SF] private GameObject[] items;

    private void Awake()
    {
        pivot.GetChild((int)type).gameObject.SetActive(true);
    }

    public void Interact(PlayerController player)
    {
        if (player.pickedItem is not null) return;

        // 일단 Instantiate로... 추후 pool로 변경
        GameObject itemObj = Instantiate(items[(int)type], pivot.position, Quaternion.identity);
        if (itemObj.TryGetComponent(out Item item)) player.AttachItem(item);
    }

    public bool BeginWork(PlayerController player) { return false; }
    public void StopWork(PlayerController player) {}
}
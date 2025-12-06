using UnityEngine;
using SF = UnityEngine.SerializeField;

public class Ingredient : MonoBehaviour
{
    [SF] private ItemType type;
    [SF] private ItemStatus doneness;
    [SF] private GameObject[] models;

    public void SetInfo(ItemType itemType, ItemStatus itemStatus, Material mat)
    {
        type = itemType;
        doneness = itemStatus;
        ActivateModel((int) itemType);
    }

    private void ActivateModel(int idx)
    {
        models[idx].SetActive(true);   
    }

    public void InitInfo()
    {
        type = 0;
        doneness = 0;
    }

    public ItemType GetItemType()
    {
        return type;
    }

    public ItemStatus GetDoneness()
    {
        return doneness;
    }
}

using UnityEngine;
using SF = UnityEngine.SerializeField;

public class Ingredient : MonoBehaviour
{
    private MeshRenderer _mesh;
    [SF] private ItemType type;
    [SF] private ItemStatus doneness;
    [SF] private Material defaultMaterial;
    [SF] private GameObject[] models;
    
    private void Awake()
    {
        _mesh = GetComponentInChildren<MeshRenderer>();
    }

    public void SetInfo(ItemType itemType, ItemStatus itemStatus, Material mat)
    {
        type = itemType;
        doneness = itemStatus;
        // _mesh.material = mat;
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
        _mesh.material = defaultMaterial;
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

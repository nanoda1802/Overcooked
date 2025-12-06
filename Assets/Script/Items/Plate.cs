using System.Collections.Generic;
using UnityEngine;
using SF = UnityEngine.SerializeField;

public class Plate : Item
{
    [Header("[Plate Only]")]
    [SF] private Transform pivot;
    [SF] private float offsetY; // 0.04f
    [SF] private GameObject ingredientPrefab;
    public bool IsInDishRack { get; set; }
    
    [Header("[UI]")]
    [SF] private MovableUIPool uiPool;
    [SF] private IngredientsInfo ingredientsInfo;

    private void OnTriggerEnter(Collider other)
    {
        if (!IsPlaced || IsInDishRack || !IsMaxDone()) return;
        if (!other.CompareTag("Item")) return;
        if (!other.TryGetComponent(out Item item) || item is Plate) return;
        if (!item.IsWellDone()) return;
        
        StackIngredient(item);
    }

    public void StackIngredient(Item item)
    {
        if (ingredientsInfo is null)
        {
            if (!uiPool.TryGetItem(out ingredientsInfo)) return;
            ingredientsInfo.ConnectWithPlate(this);
        }
        
        if (ingredientsInfo.IsFull()) return;
        if (item.type is ItemType.Bun && ingredientsInfo.HasBun) return;
        
        item.Deactivate();
        
        GameObject ingObj = Instantiate(ingredientPrefab, pivot); // [임시]
        if (!ingObj.TryGetComponent(out Ingredient ing))
        {
            Destroy(ingObj);
            return;
        }
        ing.SetInfo(item.type,item.doneness,item.Mesh.material); 
        
        ingredientsInfo.AddIngredient(ing);
        SetLocalPos(item.type, ingObj);
    }

    public bool HasIngredient()
    {
        return ingredientsInfo is not null && ingredientsInfo.GetIngredientCount() > 0;
    }

    public List<Ingredient> GetIngredients()
    {
        return ingredientsInfo.GetIngredientList();
    }

    private void SetLocalPos(ItemType itemType, GameObject ingObj)
    {
        if (itemType is ItemType.Bun)
        {
            ingObj.transform.localPosition += offsetY * Vector3.up;
        }
        else
        {
            int floor = ingredientsInfo.HasBun
                ? ingredientsInfo.GetIngredientCount()
                : ingredientsInfo.GetIngredientCount() + 1;
            ingObj.transform.localPosition += (offsetY * floor) * Vector3.up;
        }
    }
    
    public void ClearPlate()
    {
        InitProgress();
        SetMaterial();

        if (ingredientsInfo is null) return;
        ingredientsInfo.Deactivate();
        ingredientsInfo = null;
    }

    public override void InitComponents(IPool<Item> pool)
    {
        base.InitComponents(pool);
        uiPool = GameObject.Find("SubCanvas").GetComponent<MovableUIPool>(); // [임시]
    }

    public override void Activate()
    {
        base.Activate();
    }

    public override void Deactivate()
    {
        ClearPlate();
        base.Deactivate();
    }
}

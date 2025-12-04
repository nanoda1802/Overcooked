using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SF = UnityEngine.SerializeField;

public class Plate : Item
{
    [Header("[Plate Only]")]
    [SF] private Transform pivot;
    [SF] private float offsetY; // 0.04f
    [SF] private GameObject ingredientPrefab;
    
    [Header("[UI]")]
    [SF] private MovableUIPool uiPool;
    [SF] private IngredientsInfo ingredientsInfo;
    
    private void Awake()
    {
        InitComponents(null);
        uiPool = GameObject.Find("SubCanvas").GetComponent<MovableUIPool>(); // [임시]
    }

    private void Start()
    {
        InitProgress();
        SetMaterial();
        DeactivateTrail();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsPlaced || !IsDone()) return;
        if (!other.CompareTag("Item")) return;
        if (!other.TryGetComponent(out Item item) || item is Plate) return;
        
        StackIngredient(item);
    }

    public void StackIngredient(Item item)
    {
        if (ingredientsInfo is null)
        {
            if (!uiPool.TryGetUI(out ingredientsInfo)) return;
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
        ingObj.transform.localPosition += (offsetY * ingredientsInfo.GetIngredientCount()) * Vector3.up;
    }

    public bool HasIngredient()
    {
        return ingredientsInfo is not null && ingredientsInfo.GetIngredientCount() > 0;
    }

    public List<Ingredient> GetIngredients()
    {
        return ingredientsInfo.GetIngredientList();
    }
    
    public void ClearPlate()
    {
        InitProgress();
        SetMaterial();
        
        ingredientsInfo.Deactivate();
        ingredientsInfo = null;
    }
}

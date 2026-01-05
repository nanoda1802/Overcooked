using System.Collections.Generic;
using UnityEngine;
using SF = UnityEngine.SerializeField;

public class Plate : Item
{
    [Header("[Plate Only]")]
    [SF] private Transform pivot;
    [SF] private MovableUIPool uiPool;
    private IngredientsInfo _ingredientsInfo;
    public bool IsInDishRack { get; set; }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Item")) return;
        if (!other.TryGetComponent(out Item item)) return;
        
        if (!IsAbleToStack(item)) return;
        StackIngredient(item);
    }

    public bool IsAbleToStack(Item targetItem)
    {
        if (!IsPlaced || IsInDishRack || !IsMaxDone()) return false;
        if (targetItem is Plate || !targetItem.IsWellDone()) return false;
        if (_ingredientsInfo is null) return true;
        
        if (_ingredientsInfo.IsFull()) return false;
        if (targetItem.Data.ItemType is ItemType.Bun && _ingredientsInfo.HasBun) return false;
        return true;
    }

    public void StackIngredient(Item item)
    {
        if (_ingredientsInfo is null)
        {
            if (!uiPool.TryGetItem(out _ingredientsInfo)) return;
            _ingredientsInfo.ConnectWithPlate(this);
        }
        
        item.Deactivate();
        
        Ingredient ingredient = Instantiate(data.IngredientPrefab, pivot); // [임시] pool로 바꿔야
        // if (!ingObj.TryGetComponent(out Ingredient ing))
        // {
        //     Destroy(ingObj);
        //     return;
        // }
        ingredient.SetInfo(item.Data.ItemType, item.CurDoneness); 
        
        _ingredientsInfo.AddIngredient(ingredient);
        SetLocalPos(item.Data.ItemType, ingredient);
    }

    public bool HasIngredient()
    {
        return _ingredientsInfo is not null && _ingredientsInfo.GetIngredientCount() > 0;
    }

    public List<Ingredient> GetIngredients()
    {
        return _ingredientsInfo.GetIngredientList();
    }

    private void SetLocalPos(ItemType itemType, Ingredient ingredient)
    {
        if (itemType is ItemType.Bun)
        {
            ingredient.transform.localPosition += data.IngredientOffsetY * Vector3.up;
        }
        else
        {
            int floor = _ingredientsInfo.HasBun
                ? _ingredientsInfo.GetIngredientCount()
                : _ingredientsInfo.GetIngredientCount() + 1;
            ingredient.transform.localPosition += (data.IngredientOffsetY * floor) * Vector3.up;
        }
    }
    
    public void ClearPlate()
    {
        InitProgress();
        SetMaterial();

        if (_ingredientsInfo is null) return;
        _ingredientsInfo.Deactivate();
        _ingredientsInfo = null;
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

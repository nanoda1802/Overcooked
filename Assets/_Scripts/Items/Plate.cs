using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using SF = UnityEngine.SerializeField;

public class Plate : Item
{
    [Header("[Plate Only]")]
    [SF] private Transform pivot;
    // [SF] private MovableUIPool uiPool;
    private IngredientsInfo _ingredientsInfo;
    public bool IsInDishRack { get; set; }
    
    public bool HasBun { get; private set; }
    [SF] private int maxIngredientCount = 5;
    private readonly List<Ingredient> _ingredientList = new List<Ingredient>(5);
    public List<Ingredient> IngredientList => _ingredientList;
    
    private ObjectPool<IngredientsInfo> _pool;
    
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
        
        if (IsFull()) return false;
        if (targetItem.Data.ItemType is ItemType.Bun && HasBun) return false;
        return true;
    }

    public void StackIngredient(Item item)
    {
        if (_ingredientsInfo is null)
        {
            // if (!uiPool.TryGetItem(out _ingredientsInfo)) return;
            _ingredientsInfo = _pool.Get();
            _ingredientsInfo.SetPlate(this);
        }
        
        item.Deactivate();
        
        Ingredient ingredient = Instantiate(data.IngredientPrefab, pivot); // [임시] pool로 바꿔야
        // if (!ingObj.TryGetComponent(out Ingredient ing))
        // {
        //     Destroy(ingObj);
        //     return;
        // }
        ingredient.SetInfo(item.Data.ItemType, item.CurDoneness); 
        
        AddIngredient(ingredient);
        SetIngredientPos(item.Data.ItemType, ingredient);
    }

    private void AddIngredient(Ingredient ingredient)
    {
        _ingredientList.Add(ingredient);
        
        ItemType type = ingredient.GetItemType();
        if (type is ItemType.Bun)
        {
            HasBun = true;
            return;
        }
        
        _ingredientsInfo.UpdateInfoUI(type);
    }
    
    public bool HasIngredient()
    {
        return _ingredientsInfo is not null && _ingredientList.Count > 0;
    }

    private bool IsFull()
    {
        return _ingredientList.Count >= maxIngredientCount;
    }
    
    // public List<Ingredient> GetIngredients()
    // {
    //     return _ingredientsInfo.GetIngredientList();
    // }

    private void SetIngredientPos(ItemType itemType, Ingredient ingredient)
    {
        if (itemType is ItemType.Bun)
        {
            ingredient.transform.localPosition += data.IngredientOffsetY * Vector3.up;
        }
        else
        {
            int floor = HasBun ? _ingredientList.Count : _ingredientList.Count + 1;
            ingredient.transform.localPosition += (data.IngredientOffsetY * floor) * Vector3.up;
        }
    }
    
    public void ClearPlate()
    {
        InitProgress();
        SetMaterial();

        foreach (Ingredient ing in _ingredientList) // [임시]
            Destroy(ing.gameObject);
        
        _ingredientList.Clear();
        HasBun = false;
        
        if (_ingredientsInfo is null) return;
        // _ingredientsInfo.Deactivate();
        _pool.Release(_ingredientsInfo);
        _ingredientsInfo = null;
    }

    public override void InitComponents(IPool<Item> pool)
    {
        base.InitComponents(pool);
        _pool = GameObject.Find("Canvas_Movable").GetComponent<MovableUIManager>().IngredientsInfoPool; // [임시]
        // uiPool = GameObject.Find("SubCanvas").GetComponent<MovableUIPool>(); // [임시]
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

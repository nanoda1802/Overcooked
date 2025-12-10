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
        if (!IsPlaced || IsInDishRack || !IsMaxDone()) return;
        if (!other.CompareTag("Item")) return;
        if (!other.TryGetComponent(out Item item) || item is Plate) return;
        if (!item.IsWellDone()) return;
        
        StackIngredient(item);
    }

    public void StackIngredient(Item item)
    {
        if (_ingredientsInfo is null)
        {
            if (!uiPool.TryGetItem(out _ingredientsInfo)) return;
            _ingredientsInfo.ConnectWithPlate(this);
        }
        
        if (_ingredientsInfo.IsFull()) return;
        if (item.Data.ItemType is ItemType.Bun && _ingredientsInfo.HasBun) return;
        
        item.Deactivate();
        
        GameObject ingObj = Instantiate(data.IngredientPrefab, pivot); // [임시]
        if (!ingObj.TryGetComponent(out Ingredient ing))
        {
            Destroy(ingObj);
            return;
        }
        ing.SetInfo(item.Data.ItemType, item.CurDoneness); 
        
        _ingredientsInfo.AddIngredient(ing);
        SetLocalPos(item.Data.ItemType, ingObj);
    }

    public bool HasIngredient()
    {
        return _ingredientsInfo is not null && _ingredientsInfo.GetIngredientCount() > 0;
    }

    public List<Ingredient> GetIngredients()
    {
        return _ingredientsInfo.GetIngredientList();
    }

    private void SetLocalPos(ItemType itemType, GameObject ingObj)
    {
        if (itemType is ItemType.Bun)
        {
            ingObj.transform.localPosition += data.IngredientOffsetY * Vector3.up;
        }
        else
        {
            int floor = _ingredientsInfo.HasBun
                ? _ingredientsInfo.GetIngredientCount()
                : _ingredientsInfo.GetIngredientCount() + 1;
            ingObj.transform.localPosition += (data.IngredientOffsetY * floor) * Vector3.up;
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

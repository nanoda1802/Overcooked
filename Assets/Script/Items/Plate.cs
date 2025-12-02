using System.Collections.Generic;
using UnityEngine;
using SF = UnityEngine.SerializeField;

public class Plate : Item
{
    // ingredient 오브젝트 생성해 피벗의 자식으로 넣고
    // item 속성도 주입 (type, doneness, material)
    // 리스트에 들어온 아이템 넣기
    // 오브젝트에 로컬 포지션 오프셋만큼 이동 offsetY * items.Count
    // 접시 더럽히기
    
    [SF] private Transform pivot;
    [SF] private float offsetY; // 0.04f
    [SF] private GameObject prefab;
    private readonly List<Ingredient> _ingredients = new(10);

    private void OnTriggerEnter(Collider other)
    {
        if (!IsPlaced || !IsDone()) return;
        if (!other.CompareTag("Item")) return;
        if (!other.TryGetComponent(out Item item) || item is Plate) return;
        
        StackIngredient(item);
    }

    public void StackIngredient(Item item)
    {
        item.SetParent(pivot); // [임시]
        item.gameObject.SetActive(false); // [임시]
        
        GameObject ingObj = Instantiate(prefab, pivot); // [임시]
        if (!ingObj.TryGetComponent(out Ingredient ing))
        {
            Destroy(ingObj);
            return;
        }
        
        ing.SetInfo(item.type,item.doneness,item.Mesh.material); 
        
        _ingredients.Add(ing);
        ingObj.transform.localPosition += (offsetY * _ingredients.Count) * Vector3.up;
    }

    public bool HasIngredient()
    {
        return _ingredients.Count > 0;
    }

    public List<Ingredient> GetIngredientInfos()
    {
        return _ingredients;
    }
    
    public void ClearPlate()
    {
        foreach (Ingredient ing in _ingredients)
        {
            ing.InitInfo();
            Destroy(ing.gameObject); // [임시]
        }
        
        _ingredients.Clear();
        
        InitProgress();
        SetMaterial();
    }
}

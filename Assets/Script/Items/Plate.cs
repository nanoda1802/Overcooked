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

    public void StackIngredient(Item item)
    {
        if (item is Plate) return;
        
        GameObject ingObj = Instantiate(prefab, pivot); // [임시]
        if (!ingObj.TryGetComponent(out Ingredient ing))
        {
            Destroy(ingObj);
            return;
        }
        
        ing.SetItemInfo(item.type,item.doneness,item.Mesh.material); 
        _ingredients.Add(ing);
        ingObj.transform.localPosition += offsetY * _ingredients.Count * Vector3.up;
        
        InitProgress();
    }

    public bool HasIngredient()
    {
        return _ingredients.Count > 0;
    }

    public int GetPlateScore(List<FoodOrder> orders)
    {
        // 주문된 음식들을 앞에서부터 순회 (조건부 선입선출이 되도록)
        int orderIdx = -1;
        for (int i = 0; i < orders.Count; i++)
        {
            if (!orders[i].IsMatchRecipe(_ingredients)) continue;
            orderIdx = i; break;
        }
        if (orderIdx < 0) return 0;
        
        int score = orders[orderIdx].CalculateScore(Time.time);
        orders.RemoveAt(orderIdx); // [임시] 추후 매니저 단에서 제거하도록 수정
        return score;
    }

    public void ClearPlate()
    {
        foreach (Ingredient ing in _ingredients)
        {
            ing.ResetInfo();
            Destroy(ing.gameObject); // [임시]
        }
        
        _ingredients.Clear();
        SetMaterial();
    }
}

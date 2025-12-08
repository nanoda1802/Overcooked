using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SF = UnityEngine.SerializeField;

[CreateAssetMenu(fileName = "MenuData", menuName = "SO/Menu")]
public class MenuData : ScriptableObject
{
    [SF] private int spriteIndex;
    [SF] private int baseScore;
    [SF] private ItemType[] recipe;
    [SF] private float duration;

    public int SpriteIndex => spriteIndex;
    public int BaseScore => baseScore;
    public ItemType[] Recipe => recipe;
    public float Duration => duration;
    
    public Dictionary<ItemType, int> GetIngredientCounts()
    {
        Dictionary<ItemType, int> ingredientCount = new Dictionary<ItemType, int>();
        
        foreach (ItemType ing in recipe)
        {
            if (ingredientCount.TryAdd(ing,1)) continue;
            ingredientCount[ing] += 1;
        }
        
        return ingredientCount;
    }
}

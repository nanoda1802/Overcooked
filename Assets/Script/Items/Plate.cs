using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SF = UnityEngine.SerializeField;

public class Plate : Item
{
    [Header("[Plate Only]")]
    [SF] private Transform pivot;
    [SF] private float offsetY; // 0.04f
    [SF] private GameObject prefab;
    [SF] private int maxIngredientCount;
    private List<Ingredient> _ingredients;
    
    [Header("[UI]")]
    [SF] private Canvas ingredientCanvas;
    [SF] private Sprite[] ingredientSprites;
    [SF] private Image[] ingredientImages;
    
    private void Awake()
    {
        InitComponents(null);
    }

    private void Start()
    {
        InitProgress();
        SetMaterial();
        DeactivateTrail();
        DeactivateIngredientUI();
        _ingredients = new List<Ingredient>(maxIngredientCount);
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
        item.Deactivate();
        
        GameObject ingObj = Instantiate(prefab, pivot); // [임시]
        if (!ingObj.TryGetComponent(out Ingredient ing))
        {
            Destroy(ingObj);
            return;
        }
        
        ing.SetInfo(item.type,item.doneness,item.Mesh.material); 
        
        _ingredients.Add(ing);
        ingObj.transform.localPosition += (offsetY * _ingredients.Count) * Vector3.up;
        UpdateIngredientUI(item.type);
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
            // ing.InitInfo();
            Destroy(ing.gameObject); // [임시]
        }
        
        _ingredients.Clear();
        
        InitProgress();
        SetMaterial();
        DeactivateIngredientUI();
    }

    private void UpdateIngredientUI(ItemType itemType)
    {
        if (_ingredients.Count >= maxIngredientCount) return;
        if (!ingredientCanvas.gameObject.activeSelf) ingredientCanvas.gameObject.SetActive(true);
        if (itemType == ItemType.Bun) return;
        ingredientImages[_ingredients.Count - 1].sprite = ingredientSprites[(int) itemType];
        ingredientImages[_ingredients.Count - 1].gameObject.SetActive(true);
    }

    private void DeactivateIngredientUI()
    {
        ingredientCanvas.gameObject.SetActive(false);
        foreach (Image img in ingredientImages) img.gameObject.SetActive(false);
    }
}

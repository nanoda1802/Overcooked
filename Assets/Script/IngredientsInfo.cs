using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;
using SF = UnityEngine.SerializeField;

public class IngredientsInfo : MonoBehaviour, IPoolable
{
    // follow
    private Plate _targetPlate;
    private RectTransform _rect;
    private Camera _mainCam;
    // ui display
    private int _displayCount;
    [SF] private float offsetY; // 30
    [SF] private Sprite[] ingredientSprites;
    [SF] private Image[] ingredientImages;
    // list
    public bool HasBun => _hasBun;
    private bool _hasBun;
    private List<Ingredient> _ingredientList;
    // pool
    private MovableUIPool _uiPool;
    
    private void LateUpdate()
    {
        if (!gameObject.activeSelf || _targetPlate is null) return;
        FollowPlate();
    }

    public void ConnectWithPlate(Plate plate)
    {
        _targetPlate = plate;
        Activate();
    }
    
    private void FollowPlate()
    {
        Vector3 screenPoint = _mainCam.WorldToScreenPoint(_targetPlate.transform.position);
        screenPoint.y += offsetY;
        _rect.position = screenPoint;
    }
    
    public void AddIngredient(Ingredient ingredient)
    {
        _ingredientList.Add(ingredient);
        
        ItemType type = ingredient.GetItemType();
        if (type is ItemType.Bun)
        {
            _hasBun = true;
            return;
        }
        
        UpdateInfoUI(type);
    }

    private void UpdateInfoUI(ItemType type)
    {
        if (_displayCount >= ingredientImages.Length) return;
        
        ingredientImages[_displayCount].sprite = ingredientSprites[(int) type];
        ingredientImages[_displayCount].gameObject.SetActive(true);
        _displayCount++;
    }

    public bool IsFull()
    {
        return _ingredientList.Count >= ingredientSprites.Length;
    }

    public int GetIngredientCount()
    {
        return _ingredientList.Count;
    }

    public List<Ingredient> GetIngredientList()
    {
        return _ingredientList;
    }
    
    public void Init(MovableUIPool pool)
    {
        _mainCam = Camera.main;
        _rect = GetComponent<RectTransform>();
        _rect.position = Vector3.zero;
        
        _ingredientList = new List<Ingredient>(ingredientSprites.Length * 2);
        
        _uiPool = pool;
    }

    public void Activate()
    {
        gameObject.SetActive(true);
        
        foreach (Image img in ingredientImages)
        {
            img.gameObject.SetActive(false);
        }
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
        
        foreach (Ingredient ing in _ingredientList) // [임시]
            Destroy(ing.gameObject);
        
        _targetPlate = null;
        
        _ingredientList.Clear();
        _hasBun = false;
        
        _displayCount = 0;
        
        _uiPool.ReturnToPool(this);
    }
}

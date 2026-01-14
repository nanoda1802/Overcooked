using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using SF = UnityEngine.SerializeField;

public class FoodOrder : MonoBehaviour
{
    private ScoreManager _scoreManager;
    private OrderManager _orderManager;
    private MenuData _menu;
    private float _timerCount;

    private bool _isTimeOut;
    
    private RectTransform _rect;
    public RectTransform Rect => _rect;
    [SF] private float tweenDuration;
    public float TweenDuration => tweenDuration;
    
    [SF] private Image foodImage;
    [SF] private Sprite[] foodSprites;
    
    [SF] private Image[] ingredientImages;
    [SF] private Sprite[] ingredientSprites;

    [SF] private Image timerFillImage;
    [SF] private Color32[] fillColors;

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        DoTransition(new Vector2(_rect.anchoredPosition.x,150f),10f,Ease.OutBack);
        _isTimeOut = false;
    }

    private void Update()
    {
        if (_isTimeOut) return;
        UpdateTimer();
    }

    public void Init(ScoreManager sm, OrderManager om)
    {
        _scoreManager = sm;
        _orderManager = om;
    }
    
    public void Activate(MenuData menu)
    {
        _menu = menu;
        _timerCount = menu.Duration;
        SetUIImages(menu.SpriteIndex);
        
        gameObject.SetActive(true);
    }
    
    public void Deactivate(bool withTween = false)
    {
        _isTimeOut = true;
        if (withTween) DoTransition(new Vector2(_rect.anchoredPosition.x,10f),150f,Ease.InBack,OnDeactivateTweenComplete);
        else gameObject.SetActive(false);
    }

    private void DoTransition(Vector2 from, float to, Ease easeMode, TweenCallback onComplete = null)
    {
        _rect.DOKill(true);
        _rect.DOAnchorPosY(to, tweenDuration).From(from).SetEase(easeMode).OnComplete(onComplete);
    }

    private void OnDeactivateTweenComplete()
    {
        gameObject.SetActive(false); 
        _orderManager.RemoveOrder(this);
    }

    private void SetUIImages(int menuIdx)
    {
        foodImage.sprite = foodSprites[menuIdx]; // [임시] 
        for (int i = 0; i < ingredientImages.Length; i++)
        {
            if (i < _menu.Recipe.Length)
            {
                ingredientImages[i].sprite = ingredientSprites[(int)_menu.Recipe[i]];
            }
            ingredientImages[i].gameObject.SetActive(i < _menu.Recipe.Length);
        }
    }

    public bool IsMatchingRecipe(List<Ingredient> ings)
    {
        Dictionary<ItemType, int> ingCounts = _menu.GetIngredientCounts();
        if (ingCounts.Values.Sum() != ings.Count) return false;
        
        foreach (Ingredient ing in ings)
        {
            ItemType type = ing.GetItemType();
            
            if (!ingCounts.TryGetValue(type, out int count)) return false;
            if (count <= 0) return false;
            if (ing.GetDoneness() != ItemStatus.WellDone) return false;
            
            ingCounts[type] -= 1;
        }
        
        return ingCounts.Values.Sum() <= 0;
    }

    private void UpdateTimer()
    {
        _timerCount -= Time.deltaTime;
        UpdateFillImage();
        if (_timerCount > 0) return;
        
        _scoreManager.UpdateScore(GetBaseScore(),-1);
        Deactivate(true);
    }

    public float CalculateTimerRatio()
    {
        return _timerCount / _menu.Duration;
    }

    public int GetBaseScore()
    {
        return _menu.BaseScore;
    }

    private void UpdateFillImage()
    {
        float ratio = CalculateTimerRatio();
        timerFillImage.fillAmount = ratio;
        timerFillImage.color = Color32.Lerp(fillColors[0],fillColors[1],ratio); // [임시]
        
        // 연두0 ~ 노랑1 -> 노랑1 ~ 빨강2
        // int idx = (int) Mathf.Clamp(ratio * fillColors.Length,0,fillColors.Length-2);
        // // 1~0.66 / 0.66~0.33 / 0.33~0
        // // 1 / fillColors.Length
        //
        // float rr = Mathf.InverseLerp(1, 0.5f, ratio);
        // Color32 smoothColor = Color32.Lerp(fillColors[idx],fillColors[idx+1],rr);
        
        // Color32 smoothColor =Color32.Lerp()
        
        // Color smoothColor = Color.Lerp(Color.red, Color.yellow, ratio);
        // smoothColor.a *= 0.4f;
        // timerFillImage.color = smoothColor;
    }
}

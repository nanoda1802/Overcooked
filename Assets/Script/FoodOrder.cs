using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using SF = UnityEngine.SerializeField;

public class FoodOrder : MonoBehaviour
{
    private ScoreManager _scoreManager;
    private OrderManager _orderManager;
    private Menu _foodInfo;
    private Dictionary<ItemType, int> _ingredientCount;
    private float _timerCount;
    private bool _isActive;

    [SF] private Image foodImage;
    [SF] private Sprite[] foodSprites;
    
    [SF] private Image[] ingredientImages;
    [SF] private Sprite[] ingredientSprites;

    [SF] private Image timerFillImage;
    [SF] private Color32[] fillColors;
    
    private void Update()
    {
        if (!_isActive) return;
        UpdateTimer();
    }

    public void Deactivate()
    {
        _isActive = false;
        gameObject.SetActive(false);
        foreach (Image img in ingredientImages)
        {
            img.gameObject.SetActive(false);
        }
    }

    public void Activate(Menu menu)
    {
        _foodInfo = menu;
        _timerCount = menu.timer;
        SetUIImages(menu.num);

        _ingredientCount = menu.GetIngredientCount();
        
        _isActive = true;
        gameObject.SetActive(true);
    }

    public void Init(ScoreManager sm, OrderManager om)
    {
        _scoreManager = sm;
        _orderManager = om;
    }

    private void SetUIImages(int menuIdx)
    {
        foodImage.sprite = foodSprites[menuIdx]; // [임시] 
        for (int i = 0; i < _foodInfo.recipe.Count; i++)
        {
            ingredientImages[i].sprite = ingredientSprites[(int)_foodInfo.recipe[i]];
            ingredientImages[i].gameObject.SetActive(true);
        }
    }

    public bool IsMatchingRecipe(List<Ingredient> ings)
    {
        if (_ingredientCount.Values.Sum() != ings.Count)
        {
            Debug.Log($"받아야할 재료는 {_ingredientCount.Values.Sum()}개인데, 받은 재료는 {ings.Count}개야!");
            return false;
        }
        
        foreach (Ingredient ing in ings)
        {
            ItemType type = ing.GetItemType();
            
            if (!_ingredientCount.TryGetValue(type, out int count))
            {
                Debug.Log($"{type}은 레시피에 포함되지 않아!");
                return false;
            }
            if (count <= 0)
            {
                Debug.Log($"{type}은 이미 충분해!");
                return false;
            }
            if (ing.GetDoneness() != ItemStatus.WellDone)
            {
                Debug.Log($"{type} 조리가 덜 됐어!");
                return false;
            }
            
            _ingredientCount[type] -= 1;
        }
        
        return _ingredientCount.Values.Sum() <= 0;
    }

    private void UpdateTimer()
    {
        _timerCount -= Time.deltaTime;
        UpdateFillImage();

        if (_timerCount <= 0)
        {
            _scoreManager.ApplyScore(GetBaseScore(),-1);
            Deactivate();
            _orderManager.RemoveOrder(this);
        }
    }

    public float CalculateTimerRatio()
    {
        return _timerCount / _foodInfo.timer;
    }

    public int GetBaseScore()
    {
        return _foodInfo.baseScore;
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

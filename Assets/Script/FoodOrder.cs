using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using SF = UnityEngine.SerializeField;

public class FoodOrder : MonoBehaviour
{
    // 다 임시 형태
    // 지금은 테스트용으로 ServingRack에서 주문들 일괄 시간 체크하고 있는데,
    // 나ㅣ중에 주문 UI 만들면 FoodOrder애ㅔ Monobehaviour 붙이고,
    // 여기서 Update로 시간 체크하고,
    // 주문 UI 오브젝트에 컴포넌트로 붙이자
    // 기본 점수
    // private int baseScore = 100;
    // // 주문 시간
    // private float orderedTime;
    // // 마감 시간
    // private float expiredTime;
    // // 시간 체크 지표
    // public float timeCheck = 0f;
    // // 레시피
    // HashSet<ItemType> tempRecipe;

    
    private ScoreManager scoreManager;
    private OrderManager orderManager;
    private Menu foodInfo;
    private Dictionary<ItemType, int> ingredientInfo;
    private float timerCount;
    private bool isActive;

    [SF] private Image foodImage;
    [SF] private Sprite[] foodSprites;
    
    [SF] private Image[] ingredientImages;
    [SF] private Sprite[] ingredientSprites;

    [SF] private Image timerFillImage;

    private void Update()
    {
        if (!isActive) return;
        UpdateTimer();
    }

    public void Deactivate()
    {
        isActive = false;
        gameObject.SetActive(false);
        foreach (Image img in ingredientImages)
        {
            img.gameObject.SetActive(false);
        }
    }

    public void Activate(Menu menu)
    {
        foodInfo = menu;
        timerCount = menu.timer;
        SetUIImages(menu.num);

        ingredientInfo = menu.GetIngredientInfo();
        
        isActive = true;
        gameObject.SetActive(true);
    }

    public void Init(ScoreManager sm, OrderManager om)
    {
        scoreManager = sm;
        orderManager = om;
    }

    private void SetUIImages(int menuIdx)
    {
        foodImage.sprite = foodSprites[menuIdx]; // [임시] 
        for (int i = 0; i < foodInfo.recipe.Count; i++)
        {
            ingredientImages[i].sprite = ingredientSprites[(int)foodInfo.recipe[i]];
            ingredientImages[i].gameObject.SetActive(true);
        }
    }

    public bool IsMatchingRecipe(List<Ingredient> ings)
    {
        Debug.Log("--필요 재료 목록--");
        int ii = 1;
        foreach (var VARIABLE in foodInfo.recipe)
        {
            Debug.Log($"{ii++}. {VARIABLE}");
        }

        foreach (KeyValuePair<ItemType,int> pair in ingredientInfo)
        {
            Debug.Log($"{pair.Key}는 {pair.Value} 개 필요함");
        }
        
        if (ingredientInfo.Values.Sum() != ings.Count)
        {
            Debug.Log($"받아야할 재료 개수 : {ingredientInfo.Values.Sum()}");
            Debug.Log($"받은 재료 개수 : {ings.Count}");
            return false; // 재료 개수가 다름
        }
        
        // 순회 고치는 중!!! activeOrderList가 이상한 거 같은데?
        foreach (Ingredient ing in ings)
        {
            ItemType type = ing.GetType();
            if (!ingredientInfo.TryGetValue(type, out int count))
            {
                Debug.Log($"{type}은 레시피에 포함되지 않아!");
                return false;
            }
            if (count <= 0)
            {
                Debug.Log($"{type}은 이미 충분해!");
                return false;
            }
            if (ing.GetDoneness() != ItemStatus.WellDone && type != ItemType.Bun)
            {
                Debug.Log($"{type} 조리가 덜 됐어!");
                return false; // 조리가 덜 됨
            }
            
            ingredientInfo[type] -= 1;
            Debug.Log("<남은 재료>");
            foreach (KeyValuePair<ItemType,int> pair in ingredientInfo)
            {
                if (pair.Value <= 0) continue;
                Debug.Log($"{pair.Key}는 {pair.Value} 개 필요함");
            }
        }
        
        return ingredientInfo.Values.Sum() <= 0;
    }

    private void UpdateTimer()
    {
        timerCount -= Time.deltaTime;
        UpdateFillImage();

        if (timerCount <= 0)
        {
            scoreManager.ApplyScore(-1);
            Deactivate();
            orderManager.RemoveOrder(this);
        }
    }

    public float CalculateTimerRatio()
    {
        return timerCount / foodInfo.timer;
    }

    private void UpdateFillImage()
    {
        float ratio = CalculateTimerRatio();
        timerFillImage.fillAmount = ratio;
        timerFillImage.color = Color.Lerp(Color.red, Color.yellow, ratio);
    }


    private void UpdateFillImageSmooth()
    {
        // 1. ratio 계산 (1.0에서 0.0으로 감소)
        float ratio = CalculateTimerRatio();
        timerFillImage.fillAmount = ratio;
        
        // 2. Color.Lerp를 사용하여 색상 보간
        // t = 1.0일 때, 시작 색상(green)이 적용됨 (ratio가 1.0)
        // t = 0.0일 때, 목표 색상(red)이 적용됨 (ratio가 0.0)
        // Lerp의 세 번째 인자는 (0~1)이므로, ratio를 그대로 t로 사용하면 됩니다.
        
        timerFillImage.color = Color.Lerp(
            Color.red,      // ratio가 0.0일 때의 색상 (타이머 끝)
            Color.green,    // ratio가 1.0일 때의 색상 (타이머 시작)
            ratio           // 보간 계수 t
        );
        // (참고: ratio를 1 - ratio로 바꿔주면 Lerp 인자의 순서를 Color.green, Color.red로 할 수도 있습니다)
        
        // ratio가 0.75 ~ 0.50 사이일 때의 Lerp 예시
        if (ratio <= 0.75f && ratio > 0.5f)
        {
            // ratio 0.75는 t=1 (Green), ratio 0.50는 t=0 (Yellow)이 되도록 변환
            float t = Mathf.InverseLerp(0.5f, 0.75f, ratio); 

            timerFillImage.color = Color.Lerp(
                Color.yellow,  // t=0일 때의 색상
                Color.green,   // t=1일 때의 색상
                t              // 구간에 맞춰 재계산된 t
            );
        }
    }
    



















    //
    // public FoodOrder(float orderedTime, HashSet<ItemType> tempRecipe)
    // {
    //     this.orderedTime = orderedTime;
    //     this.tempRecipe = tempRecipe;
    //     expiredTime = orderedTime + 60f;
    // }
    //
    // // 레시피 비교
    // public bool IsMatchRecipe(List<Ingredient> ingredients)
    // {
    //     if (tempRecipe.Count != ingredients.Count)
    //     {
    //         Debug.Log("재료 개수가 달러!");
    //         return false;
    //     }
    //
    //     foreach (var ing in ingredients)
    //     {
    //         if (ing.GetDoneness() != ItemStatus.WellDone && ing.GetType() != ItemType.Bun)
    //         {
    //             Debug.Log($"덜 익었어!");
    //             return false;
    //         }
    //
    //         if (!tempRecipe.Contains(ing.GetType()))
    //         {
    //             Debug.Log("레시피에 없는 재료여!");
    //             return false;
    //         }
    //     }
    //
    //     return true;
    // }
    //
    // // 만료 여부 확인
    // public bool IsExpired(out int score)
    // {
    //     if(orderedTime + timeCheck > expiredTime) Debug.Log($"주문 만료! : {orderedTime}");
    //     score = baseScore;
    //     return orderedTime + timeCheck > expiredTime;
    // }
    //
    // // 점수 계산
    // public int CalculateScore(float servedTime)
    // {
    //     return (int)(baseScore * (2 - (servedTime - orderedTime) / (expiredTime)));
    // }
}

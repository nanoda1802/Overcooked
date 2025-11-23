using System.Collections.Generic;
using UnityEngine;

public class FoodOrder
{
    // 다 임시 형태
    // 지금은 테스트용으로 ServingRack에서 주문들 일괄 시간 체크하고 있는데,
    // 나ㅣ중에 주문 UI 만들면 FoodOrder애ㅔ Monobehaviour 붙이고,
    // 여기서 Update로 시간 체크하고,
    // 주문 UI 오브젝트에 컴포넌트로 붙이자
    // 기본 점수
    private int baseScore = 100;
    // 주문 시간
    private float orderedTime;
    // 마감 시간
    private float expiredTime;
    // 시간 체크 지표
    public float timeCheck = 0f;
    // 레시피
    HashSet<ItemType> recipe;

    public FoodOrder(float orderedTime, HashSet<ItemType> recipe)
    {
        this.orderedTime = orderedTime;
        this.recipe = recipe;
        expiredTime = orderedTime + 60f;
    }

    // 레시피 비교
    public bool IsMatchRecipe(List<Ingredient> ingredients)
    {
        if (recipe.Count != ingredients.Count)
        {
            Debug.Log("재료 개수가 달러!");
            return false;
        }

        foreach (var ing in ingredients)
        {
            if (ing.GetDoneness() != ItemStatus.WellDone && ing.GetType() != ItemType.Bun)
            {
                Debug.Log($"덜 익었어!");
                return false;
            }

            if (!recipe.Contains(ing.GetType()))
            {
                Debug.Log("레시피에 없는 재료여!");
                return false;
            }
        }

        return true;
    }

    // 만료 여부 확인
    public bool IsExpired(out int score)
    {
        if(orderedTime + timeCheck > expiredTime) Debug.Log($"주문 만료! : {orderedTime}");
        score = baseScore;
        return orderedTime + timeCheck > expiredTime;
    }

    // 점수 계산
    public int CalculateScore(float servedTime)
    {
        return (int)(baseScore * (2 - (servedTime - orderedTime) / (expiredTime)));
    }
}

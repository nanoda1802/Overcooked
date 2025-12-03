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
    [SF] private Color32[] fillColors;
    
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
        if (ingredientInfo.Values.Sum() != ings.Count)
        {
            Debug.Log($"받아야할 재료는 {ingredientInfo.Values.Sum()}개인데, 받은 재료는 {ings.Count}개야!");
            return false;
        }
        
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
            if (ing.GetDoneness() != ItemStatus.WellDone)
            {
                Debug.Log($"{type} 조리가 덜 됐어!");
                return false;
            }
            
            ingredientInfo[type] -= 1;
        }
        
        return ingredientInfo.Values.Sum() <= 0;
    }

    private void UpdateTimer()
    {
        timerCount -= Time.deltaTime;
        UpdateFillImage();

        if (timerCount <= 0)
        {
            scoreManager.ApplyScore(GetBaseScore(),-1);
            Deactivate();
            orderManager.RemoveOrder(this);
        }
    }

    public float CalculateTimerRatio()
    {
        return timerCount / foodInfo.timer;
    }

    public int GetBaseScore()
    {
        return foodInfo.baseScore;
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

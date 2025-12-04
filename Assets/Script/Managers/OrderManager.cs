using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using SF = UnityEngine.SerializeField;

public struct Menu // [임시]
{
    public int num;
    public int baseScore;
    public List<ItemType> recipe;
    public float timer;

    public Menu(int num, int baseScore, List<ItemType> recipe, float timer)
    {
        this.num = num;
        this.baseScore = baseScore;
        this.recipe = recipe;
        this.timer = timer;
    }
    
    public Dictionary<ItemType, int> GetIngredientCount()
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

public class OrderManager : MonoBehaviour
{
    [SF] private ScoreManager scoreManager;
    
    [SF] private int maxOrderCount;
    private List<FoodOrder> activeOrderList;
    private List<Menu> availableMenu; // 임시

    [SF] private float newOrderInterval;
    [SF] private float intervalCount;

    [SF] private Transform orderGroupUI;
    private List<FoodOrder> orderGroupChilds;

    private void Awake()
    {
        Init(); // [임시] StageManager에서 호출돼야 함
    }

    private void Update()
    {
        UpdateOrderInterval();
    }

    public void Init() // [임시] StageManager에게 데이터 받아야 함
    {
        maxOrderCount = 4;
        activeOrderList = new List<FoodOrder>(maxOrderCount);
        
        newOrderInterval = 10;
        intervalCount = newOrderInterval;

        orderGroupChilds =  new List<FoodOrder>(maxOrderCount);
        for (int i = 0; i < orderGroupUI.childCount; i++)
        {
            if (!orderGroupUI.GetChild(i).TryGetComponent(out FoodOrder order)) continue;
            orderGroupChilds.Add(order);
            order.Init(scoreManager,this);
            order.Deactivate();
        }
        
        availableMenu = new List<Menu>() // [임시]...
        {
            new Menu(0,150,new List<ItemType>{ItemType.Bun,ItemType.Cabbage,ItemType.Tomato,ItemType.Meat,ItemType.Cheese},60),
            new Menu(1,120,new List<ItemType>{ItemType.Bun,ItemType.Cabbage,ItemType.Tomato},45),
            new Menu(2,100,new List<ItemType>{ItemType.Bun,ItemType.Meat,ItemType.Cheese,ItemType.Meat},50)
        };
    }

    public bool HasActiveOrder()
    {
        return activeOrderList.Count > 0;
    }

    private void AddOrder() 
    {
        FoodOrder order = null;
        int minIndex = maxOrderCount;
        
        foreach (FoodOrder child in orderGroupChilds)      
        {
            if (child.gameObject.activeSelf) continue;

            int siblingIndex = child.transform.GetSiblingIndex();
            if (minIndex <= siblingIndex) continue;
            
            minIndex = siblingIndex;
            order = child;
        }

        if (order is null) return;
        int randomNum = Random.Range(0, availableMenu.Count);
        order.Activate(availableMenu[randomNum]);
        activeOrderList.Add(order);
    }

    public void RemoveOrder(FoodOrder order)
    {
        activeOrderList.Remove(order);
        CycleOrderGroupUI(order);
    }

    public bool FindMatchingOrder(List<Ingredient> ings, out int baseScore, out float remainingTimeRatio)
    {
        remainingTimeRatio = -1;
        baseScore = 0;
        
        foreach (var order in activeOrderList)
        {
            if(!order.IsMatchingRecipe(ings)) continue;
            remainingTimeRatio = order.CalculateTimerRatio();
            baseScore = order.GetBaseScore();
            order.Deactivate();
            RemoveOrder(order);
            break;
        }
        
        return remainingTimeRatio > 0;
    }

    private void UpdateOrderInterval()
    {
        intervalCount -= Time.deltaTime;
        
        if (intervalCount > 0) return;
        intervalCount = newOrderInterval;
        
        if (activeOrderList.Count >= maxOrderCount) return;
        AddOrder();
    }

    private void CycleOrderGroupUI(FoodOrder order)
    {
        order.transform.SetAsLastSibling();
    }
}

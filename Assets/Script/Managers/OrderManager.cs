using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using SF = UnityEngine.SerializeField;

public struct Menu // [임시]
{
    public int num;
    public List<ItemType> recipe;
    public float timer;

    public Menu(int num, List<ItemType> recipe, float timer)
    {
        this.num = num;
        this.recipe = recipe;
        this.timer = timer;
    }
    
    public Dictionary<ItemType, int> GetIngredientInfo()
    {
        Dictionary<ItemType, int> ingredientInfo = new Dictionary<ItemType, int>();
        
        foreach (ItemType ing in recipe)
        {
            if (ingredientInfo.TryAdd(ing,1)) continue;
            ingredientInfo[ing] += 1;
        }
        
        return ingredientInfo;
    }
}

public class OrderManager : MonoBehaviour
{
    [SF] private ScoreManager scoreManager;
    
    private int maxOrderCount;
    private List<FoodOrder> activeOrderList;
    private List<Menu> availableMenu; // 임시

    private float newOrderInterval;
    private float intervalCount;

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
            new Menu(0,new List<ItemType>{ItemType.Bun,ItemType.Cabbage,ItemType.Tomato,ItemType.Meat,ItemType.Cheese},45),
            new Menu(1,new List<ItemType>{ItemType.Bun,ItemType.Cabbage,ItemType.Tomato},45),
            new Menu(2,new List<ItemType>{ItemType.Bun,ItemType.Meat,ItemType.Cheese,ItemType.Meat},45)
        };
    }

    public bool HasActiveOrder()
    {
        return activeOrderList.Count > 0;
    }

    private void AddOrder() 
    {
        int randomNum = Random.Range(0, availableMenu.Count);

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
        order.Activate(availableMenu[randomNum]);
        activeOrderList.Add(order);
    }

    public void RemoveOrder(FoodOrder order)
    {
        activeOrderList.Remove(order);
        CycleOrderGroupUI(order);
    }

    public bool FindMatchingOrder(List<Ingredient> ings, out float remainingTimeRatio)
    {
        remainingTimeRatio = -1;

        for (int i = 0; i < activeOrderList.Count; i++)
        {
            Debug.Log($"~~~ {i+1} 번 주문 ~~~");
            
            FoodOrder order = activeOrderList[i];
            if(!order.IsMatchingRecipe(ings)) continue;
            remainingTimeRatio = order.CalculateTimerRatio();
            order.Deactivate();
            RemoveOrder(order);
            break;
        }

        // foreach (FoodOrder order in activeOrderList)
        // {
        //     if(!order.IsMatchingRecipe(ings)) continue;
        //     remainingTimeRatio = order.CalculateTimerRatio();
        //     order.Deactivate();
        //     RemoveOrder(order);
        //     break;
        // }
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

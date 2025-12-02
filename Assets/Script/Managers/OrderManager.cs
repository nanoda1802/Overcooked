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
}

public class OrderManager : MonoBehaviour
{
    private ScoreManager scoreManager;
    
    private int maxOrderCount;
    private List<FoodOrder> orderList;
    private List<Menu> availableMenu; // 임시

    private float newOrderInterval;
    private float intervalCount;

    [SF] private Transform orderGroupUI;
    [SF] private GameObject orderUI;

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
        orderList = new List<FoodOrder>(maxOrderCount);
        newOrderInterval = 10;
        intervalCount = newOrderInterval;
        
        availableMenu = new List<Menu>() // [임시]...
        {
            new Menu(0,new List<ItemType>{ItemType.Cabbage,ItemType.Tomato,ItemType.Meat,ItemType.Cheese},45),
            new Menu(1,new List<ItemType>{ItemType.Cabbage,ItemType.Tomato,ItemType.Cabbage,ItemType.Tomato},45),
            new Menu(2,new List<ItemType>{ItemType.Meat,ItemType.Cheese,ItemType.Meat,ItemType.Cheese},45)
        };
    }

    // 이거 해상도 바꾸니까 박살나네... orderList 캔버스에 미리 ui 네 개 두고 값들을 갱신해줘야겠는데
    // 비활성화 후 자식 순서도 바꿔줘야 레이아웃이 바뀐다...
    private void AddOrder() 
    {
        int randomNum = Random.Range(0, availableMenu.Count);
        
        GameObject ui = Instantiate(orderUI); // [임시] 추후 pool로
        if (ui.TryGetComponent(out FoodOrder order))
        {
            order.Init(scoreManager,this,availableMenu[randomNum]);
            order.transform.SetParent(orderGroupUI);
            orderList.Add(order);
        }
    }

    public void RemoveOrder(FoodOrder order)
    {
        orderList.Remove(order);
        Destroy(order.gameObject); // [임시] 추후 pool로
    }

    private bool FindMatchingOrder(List<Ingredient> ings)
    {
        // orderList를 순회하며 맞는 주문 찾기    
        return false;
    }

    private void UpdateOrderInterval()
    {
        intervalCount -= Time.deltaTime;
        
        if (intervalCount > 0) return;
        intervalCount = newOrderInterval;
        
        if (orderList.Count >= maxOrderCount) return;
        AddOrder();
    }
}

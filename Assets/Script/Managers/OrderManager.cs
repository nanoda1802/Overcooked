using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using SF = UnityEngine.SerializeField;

public class OrderManager : MonoBehaviour
{
    /* 참조 */
    private ScoreManager _scoreManager;
    private OrderInfoData _orderInfo;
    /* 주문 생성 */
    private List<FoodOrder> _activeOrderList;
    private float _intervalCount;
    /* UI */
    [SF] private Transform orderGroupUI;
    private List<FoodOrder> _orderGroupChildren;

    private void Update()
    {
        UpdateOrderInterval();
    }

    public void Init(ScoreManager sm, OrderInfoData data)
    {
        _scoreManager = sm;
        _orderInfo = data;
        
        _activeOrderList = new List<FoodOrder>(_orderInfo.MaxOrderCount);
        _intervalCount = _orderInfo.NewOrderInterval;

        _orderGroupChildren =  new List<FoodOrder>(_orderInfo.MaxOrderCount);
        for (int i = 0; i < orderGroupUI.childCount; i++)
        {
            if (!orderGroupUI.GetChild(i).TryGetComponent(out FoodOrder order)) continue;
            _orderGroupChildren.Add(order);
            order.Init(_scoreManager,this);
            order.Deactivate();
        }
    }

    public bool HasActiveOrder()
    {
        return _activeOrderList.Count > 0;
    }

    private void AddOrder() 
    {
        FoodOrder order = null;
        int minIndex = _orderInfo.MaxOrderCount;
        
        foreach (FoodOrder child in _orderGroupChildren)      
        {
            if (child.gameObject.activeSelf) continue;

            int siblingIndex = child.transform.GetSiblingIndex();
            if (minIndex <= siblingIndex) continue;
            
            minIndex = siblingIndex;
            order = child;
        }

        if (order is null) return;
        int randomIdx = Random.Range(0, _orderInfo.AvailableMenu.Length);
        order.Activate(_orderInfo.AvailableMenu[randomIdx]);
        _activeOrderList.Add(order);
    }

    public void RemoveOrder(FoodOrder order)
    {
        _activeOrderList.Remove(order);
        CycleOrderGroupUIs(order);
    }

    public bool FindMatchingOrder(List<Ingredient> ings, out int baseScore, out float remainingTimeRatio)
    {
        remainingTimeRatio = -1;
        baseScore = 0;
        
        foreach (var order in _activeOrderList)
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
        _intervalCount -= Time.deltaTime;
        if (_intervalCount > 0) return; 
        
        _intervalCount = _orderInfo.NewOrderInterval;
        if (_activeOrderList.Count < _orderInfo.MaxOrderCount) AddOrder();
    }

    private void CycleOrderGroupUIs(FoodOrder order)
    {
        order.transform.SetAsLastSibling();
    }
}

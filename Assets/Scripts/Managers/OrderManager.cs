using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;
using SF = UnityEngine.SerializeField;

public class OrderManager : MonoBehaviour, IManager
{
    /* 참조 */
    private ScoreManager _scoreManager;
    private OrderInfoData _orderInfo;
    private StageResultData _stageResult;
    /* 주문 생성 */
    private List<FoodOrder> _activeOrderList;
    private float _intervalCount;
    [SF] private AudioClip newOrderSoundClip; // [임시]
    /* UI */
    [SF] private Transform orderGroupUI;
    [SF] private int[] orderPosXs;
    private List<FoodOrder> _orderGroupChildren;

    private void Update()
    {
        if (!gameObject.activeSelf) return;
        UpdateOrderInterval();
    }

    public void Init(InStageManager sm)
    {
        _scoreManager = sm.ScoreManager;
        _orderInfo = sm.StageInfo.OrderInfoData;
        _stageResult = sm.StageResult;
        
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

    public void Deinit()
    {
        foreach (FoodOrder order in _activeOrderList)
        {
            order.Deactivate();
        }
        gameObject.SetActive(false);
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
        
        _stageResult.CountTotalOrder();
        // [sfx] 신규 주문 소리
        GameManager.Instance.SoundManager.PlaySfx(newOrderSoundClip);
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
            order.Deactivate(true);
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
        
        for (int i = 0; i < orderGroupUI.childCount; i++)
        {
            Vector3 curPos = _orderGroupChildren[i].Rect.localPosition;
            float targetPosX = orderPosXs[_orderGroupChildren[i].transform.GetSiblingIndex()];
            
            float diff = Mathf.Abs(curPos.x - targetPosX);
            if (diff < 0.01f) continue;
            
            if (!_orderGroupChildren[i].gameObject.activeSelf)
            {
                _orderGroupChildren[i].Rect.localPosition = new Vector3(targetPosX,curPos.y,curPos.z);
            }
            else
            {
                _orderGroupChildren[i].Rect.DOAnchorPosX(targetPosX, order.TweenDuration).SetEase(Ease.OutBack);
            }
        }
    }
}

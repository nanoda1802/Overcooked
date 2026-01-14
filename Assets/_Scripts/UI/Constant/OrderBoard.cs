using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sfx;
using UnityEngine;
using SF = UnityEngine.SerializeField;

public class OrderBoard : MonoBehaviour
{
    private OrderInfoData _orderInfo;
    private List<FoodOrder> _orderGroupChildren;
    [SF] private int[] orderPosXs;
    [SF] private SfxInfo newOrderSfx; // [임시]
    
    public void Init(StageManager sm, OrderManager om)
    {
        _orderInfo = sm.StageInfo.OrderInfoData;
        _orderGroupChildren = new List<FoodOrder>(transform.childCount);

        foreach (Transform child in transform)
        {
            if (!child.TryGetComponent(out FoodOrder order)) continue;
            _orderGroupChildren.Add(order);
            order.Init(sm.ScoreManager,om);
            order.Deactivate();
        }
    }
    
    public bool FindOrderUI(out FoodOrder order) 
    {
        order = null;
        int minIndex = _orderInfo.MaxOrderCount;
        
        foreach (FoodOrder child in _orderGroupChildren)      
        {
            if (child.gameObject.activeSelf) continue;

            int siblingIndex = child.transform.GetSiblingIndex();
            if (minIndex <= siblingIndex) continue;
            
            minIndex = siblingIndex;
            order = child;
        }

        if (order is null) return false;
        
        int randomIdx = Random.Range(0, _orderInfo.AvailableMenu.Length);
        order.Activate(_orderInfo.AvailableMenu[randomIdx]);
        
        GameManager.Instance.SoundManager.BuildSfx().WithSfxInfo(newOrderSfx).Play();
        return true;
    }
    
    public void CycleOrderGroupUIs(FoodOrder order)
    {
        order.transform.SetAsLastSibling();

        foreach (FoodOrder orderUI in _orderGroupChildren)       
        {
            Vector3 curPos = orderUI.Rect.localPosition;
            float targetPosX = orderPosXs[orderUI.transform.GetSiblingIndex()];
            
            float diff = Mathf.Abs(curPos.x - targetPosX);
            if (diff < 0.01f) continue;
            
            if (!orderUI.gameObject.activeSelf)
            {
                orderUI.Rect.localPosition = new Vector3(targetPosX,curPos.y,curPos.z);
            }
            else
            {
                orderUI.Rect.DOAnchorPosX(targetPosX, order.TweenDuration).SetEase(Ease.OutBack);
            }
        }
    }
}

using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using SF = UnityEngine.SerializeField;

public class CustomSlider : MonoBehaviour, IBeginDragHandler,IDragHandler,IEndDragHandler,IPointerDownHandler,IScrollHandler
{
    private int _instanceId;
    public event Action<float> OnValueChanged;
    [Range(0,1)] private float _value;

    private RectTransform _sliderRect;
    [SF] private RectTransform handleAreaRect;
    [SF] private float handleWidthRatio; // 0.035f
    
    [SF] private Image fillImg;
    [SF] private Image handleImg;
    [SF] private RectTransform handleRect;

    private Color _handleOriginalColor;
    private Vector3 _handleOriginalLocalScale;
    
    [SF,Range(0,0.1f)] private float scrollSpeed = 0.015f;
    [SF,Range(0,2)] private float handleSizeModifier = 0.98f;
    [SF,Range(0,1)] private float handleColorModifier = 0.7f;
    [SF,Range(0,1)] private float tweenDuration = 0.15f;

    private Sequence _dragSeq;
    private Sequence _scrollSeq;
    
    private void Awake()
    {
        _instanceId = gameObject.GetInstanceID();
        _sliderRect = GetComponent<RectTransform>();
        
        _handleOriginalColor = handleImg.color;
        _handleOriginalLocalScale = handleRect.localScale;
        
        AdjustHandleSizeAndArea();
    }

    private void OnDisable()
    {
        DOTween.Kill(_instanceId);
        _dragSeq = _scrollSeq = null;
        handleImg.color = _handleOriginalColor;
        handleRect.localScale = _handleOriginalLocalScale;
    }

    public void SubscribeEvent(Action<float> action)
    {
        OnValueChanged += action;
    }

    public void UnsubscribeEvent(Action<float> action)
    {
        OnValueChanged -= action;
    }

    private void AdjustHandleSizeAndArea()
    {
        handleRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,_sliderRect.sizeDelta.x * handleWidthRatio);
        float halfWidth = handleRect.rect.width * 0.5f;
        handleAreaRect.offsetMin = new Vector2(halfWidth, 0);
        handleAreaRect.offsetMax = new Vector2(-halfWidth, 0);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _dragSeq?.Kill();
        _dragSeq = DOTween.Sequence();
        
        Color targetColor = _handleOriginalColor * handleColorModifier;
        targetColor.a = 1; 

        _dragSeq.Append(handleImg.DOColor(targetColor, tweenDuration).SetEase(Ease.OutQuad))
            .Join(handleRect.DOScale(_handleOriginalLocalScale * handleSizeModifier, tweenDuration).SetEase(Ease.OutQuad))
            .SetUpdate(true)
            .SetId(_instanceId)
            .OnKill(() => _dragSeq = null);
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        bool isCursorOnRect = RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _sliderRect, // 비교할 대상의 RectTransform
            eventData.position, // 커서의 스크린 좌표 (eventData.delta는 이전 프레임 대비 현 프레임의 이동 벡터여)
            null, // Overlay 모드라 불필요
            out Vector2 localPoint); // localPoint는 RectTransform의 pivot으로부터의 상대적 좌표

        if (!isCursorOnRect) return;
        
        Rect rect = _sliderRect.rect; // RectTransform의 사각형 성질을 갖는 구조체 (너비, 높이, 모서리 등등)
        float xRatio = Mathf.Clamp01((localPoint.x - rect.xMin) / rect.width);
        
        SyncSliderElements(xRatio);
    }
    
    public void OnEndDrag(PointerEventData eventData)
    {
        _dragSeq?.Kill();
        _dragSeq = DOTween.Sequence();

        _dragSeq.Append(handleImg.DOColor(_handleOriginalColor, tweenDuration).SetEase(Ease.OutQuad))
            .Join(handleRect.DOScale(_handleOriginalLocalScale, tweenDuration).SetEase(Ease.OutQuad))
            .SetUpdate(true)
            .SetId(_instanceId)
            .OnKill(OnKillEndDragSequence);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Down 이벤트는 Press 이전이기 때문에 아직 pointer가 press한 대상은 당연히 null 인 것...!
        // if (eventData.pointerPress == handleRect.gameObject) return;
        if (eventData.pointerCurrentRaycast.gameObject == handleRect.gameObject) return;
        OnDrag(eventData);
        OnValueChanged?.Invoke(_value);
    }
    
    public void OnScroll(PointerEventData eventData) // [임시] 시퀀스 갱신이 잦음... 스크롤 값만 받고 트윈은 한 번에? 다만 반응성을...
    {
        // [중요] 스크롤 호출 간격 대략 0.02 (Time.time 값 비교 기준)
        
        /* 실패 사례 박제 */
        // Vector2 targetAnchor = rectHandle.anchorMax; // 어차피 Min도 같은 값으로 갱신할 겨
        // float targetValue = Mathf.Clamp01(targetAnchor.x + scrollSpeed * eventData.scrollDelta.y);
        // targetAnchor.x = targetValue;
        // Right, Stretch 된 Rect기 때문에, AnchorMax의 Y값은 1, AnchorMin의 Y값은 0이라 다르다! (위아래로 딱 붙는 Rect란 뜻)
        // 근데 x 값이 같을 거라고 저렇게 해버렸기 때문에 Stretch가 풀렸고, 두 Anchor의 Y가 모두 1이라 윗변과 밑변이 붙어버린, 즉 height가 0인 사각형이 된 것...
        
        _scrollSeq?.Kill();
        _scrollSeq = DOTween.Sequence();
        
        Vector2 targetAnchorMax = handleRect.anchorMax;
        Vector2 targetAnchorMin = handleRect.anchorMin;
        float targetValue = Mathf.Clamp01(targetAnchorMax.x + scrollSpeed * eventData.scrollDelta.y);
        targetAnchorMax.x = targetAnchorMin.x = targetValue;
        
        _scrollSeq.Append(handleRect.DOAnchorMax(targetAnchorMax, 0.01f)) // [임시] 적절한 duration 찾아보기
            .Join(handleRect.DOAnchorMin(targetAnchorMin, 0.01f))
            .Join(fillImg.DOFillAmount(targetValue, 0.01f))
            .SetEase(Ease.OutQuad)
            .SetUpdate(true)
            .SetId(_instanceId)
            .OnKill(()=>OnKillScrollSequence(targetValue));
    }

    private void OnKillScrollSequence(float targetValue)
    {
        _scrollSeq = null;
        SyncSliderElements(targetValue); // 혹시 모를 미세한 차이 방지
        OnValueChanged?.Invoke(_value);
    }

    private void OnKillEndDragSequence()
    {
        _dragSeq = null;
        OnValueChanged?.Invoke(_value);
    }

    public void SyncSliderElements(float value)
    {
        Vector2 targetAnchorMax = handleRect.anchorMax;
        Vector2 targetAnchorMin = handleRect.anchorMin;
        targetAnchorMax.x = targetAnchorMin.x = value;
        handleRect.anchorMin = targetAnchorMin; 
        handleRect.anchorMax = targetAnchorMax;
        
        _value = fillImg.fillAmount = value;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using SF = UnityEngine.SerializeField;

public class CustomButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private RectTransform _rect;
    private Image _img;
    private event Action OnClicked;

    // private Vector2 _originalSizeDelta; 자체 크기를 조작하는 거라 layout rebuild 발생
    private Vector3 _originalLocalScale;
    private Color _originalColor;

    [SF,Range(1,2)] private float btnSizeModifier = 1.02f;
    [SF,Range(0,1)] private float btnColorModifier = 0.7f;
    [SF,Range(0,1)] private float tweenDuration = 0.15f;
    
    private Tween _hoverEnterTween;
    private Tween _hoverExitTween;
    private Tween _clickTween;
    
    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
        _img = GetComponent<Image>();
        
        // [1] useSafeMode : 만약 트윈의 대상 오브젝트가 파괴되는 예외 상황 등이 발생해도 안전하게 처리됨 (매 프레임 트윈 작업 전 대상 상태 체크하기 때문 -> 미세한 성능 부하)
        // [2] recycleAllByDefault : 트윈이 사용 후 파괴되는 것이 아니라 풀링되기 때문에, GC 호출 수를 줄일 수 있음
        
        // 문제는 필드에 트윈을 할당해 활용하던 경우
        /*  [ 나쁜 사례 ]
            Tween prevTween = transform.DOMove(Vector3.zero, 0.5f); // DOMove를 마친 트윈은 풀에 가있음
            prevTween.Kill(); // 하지만 prevTween은 여전히 풀에 있는 트윈을 참조하기 때문에, 해당 트윈이 다른 곳에서 재사용되다가 죽어버릴 수 있음
        */
        /*  [ 좋은 사례 ]
            Tween prevTween;
            prevTween = transform.DOMove(Vector3.zero, 0.5f).OnKill(() => prevTween = null); // 작업을 마친 트윈이 풀로 돌아갈 때, 할당됐던 변수의 참조는 초기화
        */
        
        // Kill이란? 해야할 DO가 없는 트윈 객체가 해제되는 것 (작업을 마쳤거나, 작업할 오브젝트가 사라졌거나, 작업이 중단됐거나)
        // RecycleAllByDefault가 true 라면 Pool에 돌아가게 되고, false라면 방치되다가 추후 GC에 수거됨
        // 기본적으로 AutoKill 속성이 true로 켜져있어서, 모든 트윈은 Complete 후 자동으로 Kill됨
        // 다만 AutoKill 해선 안 되는 개별 트윈의 경우, SetAutoKill을 체이닝해 변경할 수 있다!
        
        // Complete란? 트윈 객체가 명령받은 DO를 성공적으로 마친 것 (설정된 시간 동안 문제없이 작업 완료) 
        
        
        // DOTween.Init(true,true).SetCapacity(100,50); // [임시] 게임매니저나 무튼 전역에서 한번만 해야함
        // DOTween.defaultAutoKill = true; // DO를 마친 트윈을 제거하는지 여부 (기본값이 true지만 공부 겸 명시...)
    }

    private void Start()
    {
        _originalLocalScale = _rect.localScale;
        _originalColor = _img.color;
    }

    public void SubscribeEvent(Action action)
    {
        OnClicked += action;
    }

    public void UnsubscribeEvent(Action action)
    {
        OnClicked -= action;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_hoverEnterTween is not null)
        {
            _hoverEnterTween?.Kill();
            OnKillHoverEnterTween();
        }
        
        _hoverEnterTween = _rect.DOScale(_originalLocalScale * btnSizeModifier, tweenDuration)
            .SetEase(Ease.InQuad)
            .OnKill(OnKillHoverEnterTween);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_hoverExitTween is not null)
        {
            _hoverExitTween?.Kill();
            OnKillHoverExitTween();
        }
        
        _hoverExitTween = _rect.DOScale(_originalLocalScale, tweenDuration)
            .SetEase(Ease.InQuad)
            .OnKill(OnKillHoverExitTween);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        GameManager.Instance.SoundManager.PlaySfx();
        
        if (_clickTween is not null) // 안 해주면 색이 돌아오지 않음... YoYo 루핑 탓에
        {
            _clickTween?.Kill();
            OnKillClickTween(); 
        }
        
        Color targetColor = _originalColor * btnColorModifier;
        targetColor.a = 1; // Color는 0~1, Color32가 1~255
        _clickTween = _img.DOColor(targetColor, tweenDuration)
            .SetLoops(2, LoopType.Yoyo)
            .SetEase(Ease.OutQuad)
            .OnKill(OnKillClickTween);
        
        OnClicked?.Invoke();
    }

    private void OnKillHoverEnterTween()
    {
        _hoverEnterTween = null;
    }

    private void OnKillHoverExitTween()
    {
        _hoverExitTween = null;
    }

    private void OnKillClickTween()
    {
        _clickTween = null;
        _img.color = _originalColor;
    }

    private void OnDisable()
    {
        _hoverEnterTween?.Kill();
        _hoverExitTween?.Kill();
        _clickTween?.Kill();
        
        OnKillHoverEnterTween();
        OnKillHoverExitTween();
        OnKillClickTween();
        
        _rect.localScale = _originalLocalScale;
    }
}

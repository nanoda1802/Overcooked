using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using SF = UnityEngine.SerializeField;

public class PopUpUI : MonoBehaviour, IPointerClickHandler
{
    [SF] protected GameObject bg;

    [SF] protected RectTransform popUpRect;
    [SF] protected float popUpTweenTargetPosY; // 1200
    [SF] protected float popUpTweenDuration; // 0.5
    private Tween _popUpTween;

    private bool _isClicked;
    protected event Action OnBgClicked;

    protected virtual void OnEnable()
    {
        _isClicked = false;
        Pop(popUpTweenTargetPosY,0,Ease.OutBack,1);
    }

    protected virtual void OnDisable()
    {
        _popUpTween?.Kill(true);
        _popUpTween = null;
    }

    public bool IsPopping()
    {
        return _popUpTween is not null;
    }

    protected void Pop(float from, float to, Ease easeMode, float overShoot = 1.7f, TweenCallback onComplete = null)
    {
        _popUpTween?.Kill(true);
        
        _popUpTween = popUpRect.DOLocalMoveY(to, popUpTweenDuration)
            .From(from)
            .SetEase(easeMode, overShoot)
            .SetUpdate(true)
            .OnComplete(onComplete)
            .OnKill(() => _popUpTween = null);
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (_isClicked) return;
        if (IsPopping()) return;
        if (eventData.pointerCurrentRaycast.gameObject != bg) return;
        
        _isClicked =  true;
        OnBgClicked?.Invoke();
    }
}

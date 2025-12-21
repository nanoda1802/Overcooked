using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using SF = UnityEngine.SerializeField;

public class PopUpUI : MonoBehaviour, IPointerClickHandler
{
    [SF] protected GameObject bg; // 생각해보니 PopUp인 UI들은 전부 백그라운드 담당이 본인 gameObject인디... 굳이...?
    
    [SF] protected RectTransform popUpRect;
    [SF] protected float popUpTweenTargetPosY; // 1200
    [SF] protected float popUpTweenDuration; // 0.5
    private Tween _popUpTween;

    private bool _isClicked;
    protected event Action OnBgClicked;

    protected virtual void OnEnable()
    {
        _isClicked = false;
        DoMoveYTransition(popUpTweenTargetPosY,0,Ease.OutBack,1,()=>_popUpTween=null);
    }

    protected virtual void OnDisable()
    {
        _popUpTween?.Kill();
        _popUpTween = null;
    }

    protected void DoMoveYTransition(float from, float to, Ease easeMode, float overShoot = 1.7f, TweenCallback onComplete = null)
    {
        if (_popUpTween is not null) return;
        _popUpTween = popUpRect.DOLocalMoveY(to, popUpTweenDuration)
            .From(from)
            .SetEase(easeMode, overShoot)
            .SetUpdate(true)
            .OnComplete(onComplete);
    }
    
    
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (_isClicked) return;
        if (_popUpTween is not null) return;
        if (eventData.pointerCurrentRaycast.gameObject != bg) return;
        
        _isClicked =  true;
        OnBgClicked?.Invoke();
    }
}

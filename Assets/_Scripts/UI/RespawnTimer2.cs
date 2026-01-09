using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using SF = UnityEngine.SerializeField;


public class RespawnTimer2 : MonoBehaviour
{
    private Canvas _canvas;
    private RectTransform _canvasRect;
    private CanvasGroup _canvasGroup;
    
    [SF] private RectTransform bgRect;
    [SF] private Text timerTxt;

    [SF] private float respawnTime = 5;
    private float _timerCount;
    private int _prevSecond;

    [SF] private Vector3 offsetY = new Vector3(0,0.01f,0);
    [SF] private Vector3 originLocalRot = new Vector3(90, 0, 0);
    [SF] private Vector3 originLocalScale = new Vector3(0.9f, 0.9f, 0.9f);
    
    private Sequence _transitionSeq;
    public event Action OnTimerDone;

    private void Update()
    {
        if (!_canvas.enabled) return;
        UpdateTimer();
    }

    public void Init()
    {
        _canvas = GetComponent<Canvas>();
        _canvasRect = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();
        Deactivate();
    }

    public void Activate(Vector3 respawnPos)
    {
        InitRectValues(respawnPos);
        InitTimerValues();
        _canvas.enabled = true;
        
        // [메모] 캔버스든 UI 요소든 SetParent는 그냥 하지마쇼 아무튼 하지마쇼
        // 아예 껐다 키는 것도 하지 마쇼 (Canvas의 enabled와 CanvasGroup의 alpha로 조절하자)
        // Rebuild가 심함ㅁㅁㅁㅁㅁㅁㅁ
        
        // _canvasRect.SetParent(parent);
        // InitRectValues();
        // gameObject.SetActive(true);

        
        // [임시] 등장 트윈
        _transitionSeq?.Kill(true);
        _transitionSeq = DOTween.Sequence()
            .Append(bgRect.DOScale(1, 0.5f).From(0))
            .Join(_canvasGroup.DOFade(1, 0.5f).From(0))
            .OnKill(OnKillTransitionSequence);
    }

    private void Deactivate()
    {
        _canvas.enabled = false;
        _canvasGroup.alpha = 0;
        // gameObject.SetActive(false);
        // _canvasRect.SetParent(_fallZone.transform);
        
        OnTimerDone = null;
        _transitionSeq?.Kill(true);
    }

    private void InitRectValues(Vector3 respawnPos)
    {
        transform.position = respawnPos + offsetY;
        // 아래는 사실 안 해도 되지만...
        _canvasRect.localRotation = Quaternion.Euler(originLocalRot);
        _canvasRect.localScale = originLocalScale;
    }

    private void InitTimerValues()
    {
        _timerCount = respawnTime;
        _prevSecond = (int)respawnTime;
        UpdateTimerText(_prevSecond);
    }

    private void UpdateTimerText(int value)
    {
        timerTxt.text = value.ToString();
    }

    private void UpdateTimer()
    {
        _timerCount -= Time.deltaTime;
        
        int curSecond = (int)_timerCount;
        
        if (_prevSecond > curSecond)
        {
            UpdateTimerText(curSecond);
            _prevSecond = curSecond;
        }

        if (_timerCount > 0) return;
        // OnTimerDone?.Invoke();
        // [임시]퇴장트윈
        _transitionSeq?.Kill(true);
        _transitionSeq = DOTween.Sequence()
            .AppendCallback(()=>OnTimerDone?.Invoke())
            .Append(bgRect.DOScale(0, 0.5f).From(1))
            .Join(_canvasGroup.DOFade(0, 0.5f).From(1))
            .OnComplete(Deactivate)
            .OnKill(OnKillTransitionSequence);
    }

    private void OnKillTransitionSequence()
    {
        _transitionSeq = null;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using SF = UnityEngine.SerializeField;


public class RespawnTimer2 : MonoBehaviour
{
    private RectTransform _canvasRect;
    private CanvasGroup _canvasGroup;
    
    [SF] private RectTransform bgRect;
    [SF] private Text timerTxt;

    [SF] private float respawnTime = 5;
    private float _timerCount;
    private bool _isTimerDone;
    
    private Sequence _transitionSeq;

    private event Action OnTimerDone;
    
    private void Awake()
    {
        _canvasRect = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    private void OnEnable()
    {
        _timerCount = respawnTime;
        _isTimerDone = false;
        
        SubscribeEvent(()=>_isTimerDone = true);
        
        // [임시] 등장 트윈
        _transitionSeq?.Kill();
        _transitionSeq = DOTween.Sequence()
            .Append(bgRect.DOScale(1, 0.5f).From(0))
            .Join(_canvasGroup.DOFade(1, 0.5f).From(0));
    }

    private void OnDisable()
    {
        // 정리
        OnTimerDone = null;
    }

    private void Update()
    {
        if (_isTimerDone) return;
        UpdateTimer();
    }

    private void UpdateTimer()
    {
        _timerCount -= Time.deltaTime;
        timerTxt.text = $"{_timerCount:F0}";

        if (_timerCount > 0) return;
        // OnTimerDone?.Invoke();
        // [임시]퇴장트윈
        _transitionSeq?.Kill();
        _transitionSeq = DOTween.Sequence()
            .AppendCallback(()=>OnTimerDone?.Invoke())
            .Append(bgRect.DOScale(0, 0.5f).From(1))
            .Join(_canvasGroup.DOFade(0, 0.5f).From(1))
            .OnComplete(()=> gameObject.SetActive(false));
    }
    
    public void SubscribeEvent(Action action)
    {
        OnTimerDone += action;
    }



}

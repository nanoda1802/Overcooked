using System;
using DG.Tweening;
using Sfx;
using UnityEngine;
using UnityEngine.UI;
using SF = UnityEngine.SerializeField;

public enum CueType
{
    Start,
    End,
    Quit
}

public class StageCue : MonoBehaviour
{
    [SF] private Text cueTxt;
    [SF] private RectTransform cueRect;

    [SF] private ClipInfo alertSfx;
    [SF] private ClipInfo stageStartSfx;
    [SF] private ClipInfo stageEndSfx;

    [SF] private string[] startCueTexts;
    [SF] private string[] endCueTexts;
    [SF] private string[] quitCueTexts;
    [SF,Range(1,2)] private float cueDuration;

    private Action _onStageStarted;
    private Action _onStageEnded;
    
    private Sequence _cueSeq;

    private void OnDisable()
    {
        _cueSeq?.Kill();
        _cueSeq = null;
        
        cueTxt.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        _onStageStarted = null;
        _onStageEnded = null;
    }

    public void Init(InStageManager inStageManager)
    {
        _onStageStarted = inStageManager.StartStage;
        _onStageEnded = inStageManager.StageResultUI.Activate;
    }

    public void Activate(CueType cueType)
    {
        string[] cueTexts = cueType switch
        {
            CueType.Start => startCueTexts,
            CueType.End => endCueTexts,
            CueType.Quit => quitCueTexts,
            _ => null
        };

        if (cueTexts is null) return;
        
        gameObject.SetActive(true); // [임시] 페이드로 변경
        DisplayCue(cueDuration, cueTexts, cueType == CueType.Start);
    }

    private void DisplayCue(float duration, string[] cueTexts, bool isStartCue)
    {
        _cueSeq?.Kill(true);
        
        _cueSeq = DOTween.Sequence();
        _cueSeq.AppendInterval(duration*0.25f)
            .AppendCallback(()=>cueTxt.gameObject.SetActive(true));
        
        for (int i = 0; i < cueTexts.Length; i++)
        {
            string cueText = cueTexts[i]; 
            bool isLastText = (i == cueTexts.Length - 1);
            ClipInfo sfx = alertSfx;
            if (isLastText) sfx = isStartCue ? stageStartSfx : stageEndSfx;

            // 여기서 바로 cueTexts[i]를 할당하면, 클로져 문제로 시퀀스가 실행될 시점엔 i가 이미 초기화돼있어서 적절한 string이 할당되지 않음.
            // 그래서 위에서 cueText란 변수에 문자열을 스냅샷으로 남기고 콜백에 전달해야함
            _cueSeq.AppendCallback(() => UpdateCueTxt(cueText, sfx))
                .Append(cueTxt.DOFade(1, duration*0.1f).From(0))
                .Join(cueRect.DOScale(1, duration*0.6f).From(0).SetEase(Ease.OutBack));

            if (isLastText) _cueSeq.Join(cueRect.DOShakeAnchorPos(duration*0.6f, 50, 10, 90,true,true,ShakeRandomnessMode.Harmonic));
            _cueSeq.Append(cueTxt.DOFade(0, duration*0.1f).SetDelay(duration*0.4f));
        }

        _cueSeq.SetUpdate(true) // timeScale 영향 X
            .OnComplete(() => OnCueComplete(isStartCue))
            .OnKill(() => _cueSeq = null);
    }

    private void OnCueComplete(bool isStageStart)
    {
        if (isStageStart) _onStageStarted?.Invoke();
        else _onStageEnded?.Invoke();
        
        gameObject.SetActive(false); // [임시] 페이드로 변경
    }

    private void UpdateCueTxt(string cueText, ClipInfo sfx)
    {
        cueTxt.text = cueText;
        GameManager.Instance.SoundManager.BuildSfx().WithSfxInfo(sfx).Play();
    }
}

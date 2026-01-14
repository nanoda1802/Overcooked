using System;
using DG.Tweening;
using Sfx;
using UnityEngine;
using UnityEngine.UI;
using SF = UnityEngine.SerializeField;

public enum CueType
{
    Start,
    Timeout,
    Quit
}

public class StageCue : MonoBehaviour
{
    [SF] private Canvas popUpCanvas;
    /* UI Elements */
    [SF] private RectTransform cueRect; // [임시] 본인 Rect이므
    [SF] private Image bg;
    [Header("[ Texts ]")]
    [SF] private Text cueTxt; // [임시] 바로 자식 Text이므
    /* Data */
    [Header("[ Cue Infos ]")]
    [SF] private string[] startCue; // [임시] 따로 SO로 뺄까?
    [SF] private string[] timeoutCue; // [임시] 따로 SO로 뺄까?
    [SF] private string[] quitCue; // [임시] 따로 SO로 뺄까?
    [SF,Range(1,2)] private float cueDuration; // [임시] 따로 SO로 뺄까?
    [Header("[ SFX ]")]
    [SF] private SfxInfo alertSfx;
    [SF] private SfxInfo stageStartSfx;
    [SF] private SfxInfo stageEndSfx;
    /* Fields */
    private Action _onStageStarted;
    private Action _onStageEnded;
    private Sequence _cueSeq;

    #region Unity Event Methods
    private void OnDisable()
    {
        _cueSeq?.Kill(true);
        _cueSeq = null;
    }

    private void OnDestroy()
    {
        _onStageStarted = null;
        _onStageEnded = null;
    }
    #endregion

    #region Initialize Methods
    public void Init(StageManager stageManager)
    {
        _onStageStarted = stageManager.StartStage;
        _onStageEnded = stageManager.StageResultUI.Activate;
    }
    #endregion

    #region UI Control Methods
    public void Activate(CueType cueType)
    {
        string[] cueTexts = cueType switch
        {
            CueType.Start => startCue,
            CueType.Timeout => timeoutCue,
            CueType.Quit => quitCue,
            _ => null
        };

        if (cueTexts is null) return;

        // if (!bg.gameObject.activeSelf) bg.gameObject.SetActive(true);
        if (!popUpCanvas.enabled) popUpCanvas.enabled = true;
        DisplayCue(cueDuration, cueTexts, cueType == CueType.Start);
    }

    private void Deactivate(bool isStageStart)
    {
        gameObject.SetActive(false);
        // bg.gameObject.SetActive(false);
        popUpCanvas.enabled = !isStageStart;
        
        if (isStageStart) _onStageStarted?.Invoke();
        else _onStageEnded?.Invoke();
    }
    
    private void DisplayCue(float duration, string[] cue, bool isStartCue)
    {
        gameObject.SetActive(true);
        
        _cueSeq?.Kill(true);
        _cueSeq = DOTween.Sequence();
        
        for (int i = 0; i < cue.Length; i++)
        {
            bool isLastText = (i == cue.Length - 1);
            string curCue = cue[i]; 
            SfxInfo curSfx = alertSfx;
            if (isLastText) curSfx = isStartCue ? stageStartSfx : stageEndSfx;

            // 여기서 바로 cueTexts[i]를 할당하면, 클로져 문제로 시퀀스가 실행될 시점엔 i가 이미 초기화돼있어서 적절한 string이 할당되지 않음.
            // 그래서 위에서 cueText란 변수에 문자열을 스냅샷으로 남기고 콜백에 전달해야함
            _cueSeq.AppendCallback(() => UpdateCueTxt(curCue, curSfx))
                .Append(cueTxt.DOFade(1, duration*0.1f).From(0))
                .Join(cueRect.DOScale(1, duration*0.6f).From(0).SetEase(Ease.OutBack));

            if (isLastText)
            {
                _cueSeq.Join(cueRect.DOShakeAnchorPos(duration * 0.6f, 50, 10, 90, true, true,
                    ShakeRandomnessMode.Harmonic));
            }

            if (!isStartCue)
            {
                _cueSeq.AppendInterval(duration*0.4f);
            }
            
            _cueSeq.Append(cueTxt.DOFade(0, duration*0.1f).SetDelay(duration*0.4f));
        }

        _cueSeq.SetUpdate(true) // timeScale 영향 X
            .OnComplete(() => Deactivate(isStartCue))
            .OnKill(() => _cueSeq = null);
    }
    
    private void UpdateCueTxt(string cueText, SfxInfo sfx)
    {
        cueTxt.text = cueText;
        GameManager.Instance.SoundManager.BuildSfx().WithSfxInfo(sfx).Play();
    }
    #endregion
}

using System;
using System.Text;
using DG.Tweening;
using Sfx;
using UnityEngine;
using UnityEngine.UI;
using SF = UnityEngine.SerializeField;

public class TimerBoard : MonoBehaviour
{
    // [정리하자ㅏㅏ]
    [SF] private RectTransform timerTxtRect;
    [SF] private Text timerTxt;
    [SF] private Color feverTimeTextColor;
    [SF] private float tweenScaleModifier;
    [SF] private float tweenDuration;
    [SF] private float feverTime;
    
    [SF] private SfxInfo alarmSfx;
    
    private Sequence _timerTxtSeq;
    
    [SF] private Image timerFillImg;
    [SF] private Color32[] fillColors;
    
    private int _prevSecond;
    private const string DefaultTimeText = "00:00";
    private const string TimeFormat = "{0:D2}:{1:D2}";
    
    private StringBuilder _stringBuilder;
    
    private void OnEnable()
    {
        _stringBuilder = new StringBuilder();
        _prevSecond = -1; // 첫 타이머 갱신 위해 필요...
    }

    private void OnDisable()
    {
        _timerTxtSeq?.Kill();
    }

    public void UpdateTimerUI(float ratio, float leftTime)
    {
        UpdateFillImage(ratio);

        TimeSpan timeSpan = TimeSpan.FromSeconds(leftTime);
        int curSecond = timeSpan.Seconds;
        if (curSecond == _prevSecond) return;
        
        _prevSecond = curSecond;
        timerTxt.text = leftTime < 0 ? DefaultTimeText : BuildTimeString(timeSpan.Minutes, curSecond);
        
        if (leftTime <= feverTime) DoTimerTxtSequence();
    }
    
    private string BuildTimeString(int m, int s)
    {
        _stringBuilder.Clear();
        _stringBuilder.AppendFormat(TimeFormat, m, s);
        return _stringBuilder.ToString();
    }
    
    private void UpdateFillImage(float ratio)
    {
        timerFillImg.fillAmount = ratio;
        timerFillImg.color = Color32.Lerp(fillColors[0],fillColors[1],ratio); // [임시]
    }
    
    private void DoTimerTxtSequence()
    {
        _timerTxtSeq?.Kill();
        
        _timerTxtSeq = DOTween.Sequence();
        _timerTxtSeq.Append(timerTxtRect.DOPunchScale(tweenScaleModifier * Vector3.one, tweenDuration * 2f, 1))
            .Join(timerTxt.DOColor(feverTimeTextColor, tweenDuration).SetLoops(2, LoopType.Yoyo))
            .JoinCallback(PlayAlarmSfx)
            .OnKill(OnKillTimerTxtSequence);
    }

    private void PlayAlarmSfx()
    {
        GameManager.Instance.SoundManager.BuildSfx().WithSfxInfo(alarmSfx).Play();
    }

    private void OnKillTimerTxtSequence()
    {
        _timerTxtSeq = null;
        timerTxtRect.localScale = Vector3.one;
        timerTxt.color = Color.white;
    }
}

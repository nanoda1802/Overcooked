using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using SF = UnityEngine.SerializeField;

public class ScoreBoard : MonoBehaviour
{
    [SF] private Image fireImg; // 지워 말어...
    
    [SF] private RectTransform scoreTxtRect;
    [SF] private Text scoreTxt;
    [SF] private Color scoreTxtOriginalColor;
    [SF] private Color addScoreTxtColor;
    [SF] private Color deductScoreTxtColor;
    
    [SF] private Text comboTxt;
    [SF] private Color[] comboTxtColors;

    [SF] private float tweenDuration;
    [SF,Range(0,2)] private float tweenScaleModifier;
    
    [SF] private AudioClip addScoreSoundClip;
    [SF] private AudioClip deductScoreSoundClip;
    
    private Sequence _scoreTxtSeq;
    private Sequence _comboTxtSeq;

    private void OnDisable()
    {
        _scoreTxtSeq?.Kill(true);
    }

    public void UpdateScore(int from, int to, bool hasPoint)
    {
        _scoreTxtSeq?.Kill(true);
        
        _scoreTxtSeq = DOTween.Sequence();
        _scoreTxtSeq.Append(scoreTxt.DOCounter(from,to, tweenDuration, false))
            .Append(scoreTxt.DOColor(hasPoint ? addScoreTxtColor : deductScoreTxtColor, tweenDuration).SetLoops(2, LoopType.Yoyo))
            .Join(scoreTxtRect.DOPunchScale(tweenScaleModifier * Vector3.one, tweenDuration * 2f,1))
            .JoinCallback(()=>GameManager.Instance.SoundManager.PlaySfx(hasPoint ? addScoreSoundClip : deductScoreSoundClip))
            .OnKill(OnKillScoreTxtSequence);
    }

    private void OnKillScoreTxtSequence()
    {
        _scoreTxtSeq = null;
        scoreTxtRect.localScale = Vector3.one;
        scoreTxt.color = scoreTxtOriginalColor;
    }

    public void UpdateCombo(int comboCnt)
    {
        comboTxt.text = $"{comboCnt} Combo";
        int colorIdx = Mathf.Clamp(comboCnt,0,comboTxtColors.Length - 1);
        comboTxt.color = comboTxtColors[colorIdx];
    }
}

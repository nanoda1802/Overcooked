using System;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using SF =  UnityEngine.SerializeField;

public class TimeManager : MonoBehaviour, IManager
{
    private StageInfoData _stageInfo;
    private float _leftTime;
    private int _prevSecond = -1;
    private StringBuilder _stringBuilder;
    
    private const string DefaultTimeText = "00:00";
    private const string TimeFormat = "{0:D2}:{1:D2}";
    
    [SF] private Text timerText;
    [SF] private Image timerFillImage;
    [SF] private Color32[] fillColors;
    
    private Action _onTimerDone;
    
    private void Update()
    {
        if (!gameObject.activeSelf) return;
        UpdateTimer();
    }
    
    public void Init(StageManager sm)
    {
        _onTimerDone += sm.FinishStage;
        _stageInfo = sm.StageInfo;
        _leftTime = _stageInfo.StageDuration;
        _prevSecond = -1; // 첫 타이머 갱신 위해 필요...
        _stringBuilder = new StringBuilder();
    }

    public void Deinit()
    {
        gameObject.SetActive(false);
    }

    private void UpdateTimer()
    {
        if (_leftTime <= 0)
        {
            _onTimerDone?.Invoke();
            return;
        }
        
        _leftTime -= Time.deltaTime;
        UpdateTimerUI();
    }

    private void UpdateTimerUI()
    {
        UpdateFillImage();

        TimeSpan timeSpan = TimeSpan.FromSeconds(_leftTime);
        int curSecond = timeSpan.Seconds;
        if (curSecond == _prevSecond) return;
        _prevSecond = curSecond;
        
        timerText.text = _leftTime < 0 ? DefaultTimeText : BuildTimerText(timeSpan.Minutes, curSecond);
    }

    private string BuildTimerText(int m, int s)
    {
        _stringBuilder.Clear();
        _stringBuilder.AppendFormat(TimeFormat, m, s);
        return _stringBuilder.ToString();
    }
    
    private float CalculateTimerRatio()
    {
        return _leftTime / _stageInfo.StageDuration;
    }
    
    private void UpdateFillImage()
    {
        float ratio = CalculateTimerRatio();
        timerFillImage.fillAmount = ratio;
        timerFillImage.color = Color32.Lerp(fillColors[0],fillColors[1],ratio); // [임시]
    }
}

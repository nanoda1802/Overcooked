using System;
using UnityEngine;
using SF =  UnityEngine.SerializeField;

public class TimeManager : MonoBehaviour, IManager
{
    private StageInfoData _stageInfo;
    private float _leftTime;

    [SF] private TimerBoard timerUI;
    
    private event Action OnTimerDone;
    
    private void Update()
    {
        if (!gameObject.activeSelf) return;
        UpdateTimer();
    }
    
    public void Init(StageManager sm)
    {
        OnTimerDone += sm.EndStage;
        _stageInfo = sm.StageInfo;
        _leftTime = _stageInfo.StageDuration;
    }

    public void Deinit()
    {
        OnTimerDone = null;
        gameObject.SetActive(false);
    }

    public void PauseTimer()
    {
        Time.timeScale = 0;
    }

    public void ResumeTimer()
    {
        Time.timeScale = 1;
    }

    private void UpdateTimer()
    {
        if (_leftTime <= 0)
        {
            // [sfx] 스테이지 타임아웃 소리
            OnTimerDone?.Invoke();
            return;
        }
        
        _leftTime -= Time.deltaTime;
        timerUI.UpdateTimerUI(CalculateTimerRatio(), _leftTime);
    }
    
    private float CalculateTimerRatio()
    {
        return _leftTime / _stageInfo.StageDuration;
    }
}

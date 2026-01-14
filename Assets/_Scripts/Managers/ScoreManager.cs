using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using SF = UnityEngine.SerializeField;

public class ScoreManager : MonoBehaviour, IManager
{
    private StageResultData _stageResult;
    private int _comboCount;

    [SF] private ScoreBoard scoreUI;
    
    public void Init(StageManager sm)
    {
        _stageResult = sm.StageResult;
        _stageResult.Init();
        
        ResetComboCount();
        scoreUI.ResetScore();
    }

    public void Deinit()
    {
        gameObject.SetActive(false);
    }

    public void UpdateScore(int baseScore, float ratio)
    {
        int point = CalculatePoint(baseScore, ratio);
        
        if (point <= 0)
        {
            ResetComboCount();
        }
        else
        {
            AddComboCount();
            _stageResult.CountDeliveredOrder();
        }
        
        scoreUI.UpdateScore(_stageResult.Score, _stageResult.ApplyPoint(point), point > 0);
    }

    private int CalculatePoint(int baseScore, float ratio)
    {
        if (ratio < 0) return (int) (baseScore * -0.5f);
        return (int) (baseScore * (1 + ratio + (_comboCount * _stageResult.ComboModifier)));
    }

    private void AddComboCount()
    {
        _comboCount++;
        scoreUI.UpdateCombo(_comboCount);
        // fireImg?.gameObject.SetActive(true);
        if (_stageResult.IsMaxCombo(_comboCount)) _stageResult.UpdateMaxCombo(_comboCount);
    }

    private void ResetComboCount()
    {
        _comboCount = 0;
        scoreUI.UpdateCombo(_comboCount);
        // fireImg?.gameObject.SetActive(false);
    }
}

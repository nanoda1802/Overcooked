using UnityEngine;
using UnityEngine.UI;
using SF = UnityEngine.SerializeField;

public class ScoreManager : MonoBehaviour, IManager
{
    private StageResultData _stageResult;
    private int _comboCount;
    
    [SF] private Text scoreTxt;
    [SF] private Text comboTxt;
    [SF] private Image fireImg;
    
    private Animator _scoreTxtAnim;
    private readonly int _addParamHash = Animator.StringToHash("Add");
    private readonly int _deductParamHash = Animator.StringToHash("Deduct");
    
    [SF] private Color[] comboTxtColors;

    public void Init(InStageManager sm)
    {
        _stageResult = sm.StageResult;
        _stageResult.Init();
        _scoreTxtAnim = scoreTxt.GetComponent<Animator>();
        ResetComboCount();
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
            _scoreTxtAnim.SetTrigger(_deductParamHash);    
        }
        else
        {
            _stageResult.CountDeliveredOrder();
            AddComboCount();
            _scoreTxtAnim.SetTrigger(_addParamHash);
        }
        
        _stageResult.ApplyPoint(point);
        scoreTxt.text = $"{_stageResult.Score}";
    }

    private int CalculatePoint(int baseScore, float ratio)
    {
        if (ratio < 0) return (int) (baseScore * -0.5f);
        return (int) (baseScore * (1 + ratio + (_comboCount * _stageResult.ComboModifier)));
    }

    private void AddComboCount()
    {
        _comboCount++;
        if (_stageResult.IsMaxCombo(_comboCount)) _stageResult.UpdateMaxCombo(_comboCount);
        
        fireImg?.gameObject.SetActive(true);
        UpdateComboText();
    }

    private void ResetComboCount()
    {
        _comboCount = 0;
        fireImg?.gameObject.SetActive(false);
        UpdateComboText();
    }

    private void UpdateComboText()
    {
        comboTxt.text = $"{_comboCount} Combo";
        int colorIdx = Mathf.Clamp(_comboCount,0,comboTxtColors.Length - 1);
        comboTxt.color = comboTxtColors[colorIdx];
    }
}

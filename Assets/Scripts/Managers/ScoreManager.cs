using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using SF = UnityEngine.SerializeField;

public class ScoreManager : MonoBehaviour, IManager
{
    private StageResultData _stageResult;
    private int _comboCount;
    
    // [임시] ScoreTxt 스크립트 분리하기 (여기 필드 추출)
    [SF] private Text scoreTxt;
    [SF] private Text comboTxt;
    [SF] private Image fireImg;
    
    [SF] private Color scoreTxtOriginalColor;
    private Animator _scoreTxtAnim;
    private readonly int _addParamHash = Animator.StringToHash("Add");
    private readonly int _deductParamHash = Animator.StringToHash("Deduct");
    
    [SF] private Color[] comboTxtColors;

    [SF] private AudioClip addScoreSoundClip; // [임시]
    [SF] private AudioClip deductScoreSoundClip; // [임시]
    
    public void Init(InStageManager sm)
    {
        _stageResult = sm.StageResult;
        _stageResult.Init();
        _scoreTxtAnim = scoreTxt.GetComponent<Animator>();
        _scoreTxtAnim.enabled = false; // [임시] 트윈 테스트중
        ResetComboCount();
    }

    public void Deinit()
    {
        gameObject.SetActive(false);
    }

    public void UpdateScore(int baseScore, float ratio)
    {
        int point = CalculatePoint(baseScore, ratio);

        Color targetColor;
        
        if (point <= 0)
        {
            // [sfx] 감점 소리
            GameManager.Instance.SoundManager.PlaySfx(deductScoreSoundClip);
            ResetComboCount();
            // _scoreTxtAnim.SetTrigger(_deductParamHash);
            targetColor = Color.red; // [임시]
        }
        else
        {
            // [sfx] 득점 소리
            GameManager.Instance.SoundManager.PlaySfx(addScoreSoundClip);
            _stageResult.CountDeliveredOrder();
            AddComboCount();
            // _scoreTxtAnim.SetTrigger(_addParamHash);
            targetColor = Color.green; // [임시]
        }
        
        // _stageResult.ApplyPoint(point);
        Sequence txtSeq = DOTween.Sequence();

        txtSeq.Append(scoreTxt.DOCounter(_stageResult.Score, _stageResult.ApplyPoint(point), 0.5f, false))
            .Append(scoreTxt.DOColor(targetColor, 0.25f))
            .Append(scoreTxt.DOColor(scoreTxtOriginalColor, 0.25f));
        // scoreTxt.text = $"{_stageResult.Score}";
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

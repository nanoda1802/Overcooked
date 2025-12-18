using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using SF = UnityEngine.SerializeField;

public class StageResult : MonoBehaviour
{
    private StageResultData _stageResult;

    private GameManager _gameManager;
    
    [SF] private Text scoreValueTxt;
    [SF] private Text maxComboValueTxt;
    [SF] private Text deliveredOrderValueTxt;
    [SF] private Text incomeValueTxt;

    public void Init(InStageManager sm)
    {
        _stageResult = sm.StageResult;
        _gameManager = sm.GameManager;
    }

    public void Activate()
    {
        gameObject.SetActive(true);
        SetTexts();
        // [sfx] 결산창 소리
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }
    
    private void SetTexts()
    {
        scoreValueTxt.text = $"{_stageResult.Score}";
        maxComboValueTxt.text = $"{_stageResult.MaxCombo}";
        deliveredOrderValueTxt.text = $"{_stageResult.DeliveredOrder} ({_stageResult.CalculateDeliverRate()*100:F0}%)";
        
        int income = _stageResult.CalculateIncome();
        incomeValueTxt.text = $"$ {income/100}.{income%100}";
    }

    public void OnRetryButton()
    {
        _gameManager.ChangeScene("InStage");
        Deactivate();
        GameManager.Instance.SoundManager.PlaySfx();
    }
    
    public void OnLobbyButton()
    {
        _gameManager.ChangeScene("OutStage");
        Deactivate();
        GameManager.Instance.SoundManager.PlaySfx();
    }
}

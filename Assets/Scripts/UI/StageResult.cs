using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using SF = UnityEngine.SerializeField;

public class StageResult : MonoBehaviour
{
    private StageResultData _stageResult;

    [SF] private RectTransform bgRect;
    [SF] private RectTransform resultRect;
    [SF] private RectTransform lobbyBtnRect;
    [SF] private RectTransform retryBtnRect;
    
    [SF] private Text scoreText;
    [SF] private Text scoreValueTxt;
    [SF] private Text maxComboTxt;
    [SF] private Text maxComboValueTxt;
    [SF] private Text deliveredOrderTxt;
    [SF] private Text deliveredOrderValueTxt;
    [SF] private Text incomeTxt;
    [SF] private Text incomeValueTxt;

    [SF] private CustomButton lobbyBtn;
    [SF] private CustomButton retryBtn;

    [SF] private Text[] tempTexts;
    [SF] private Text[] tempValueTexts;
    [SF] private RectTransform[] tempValueTextRects;
    private string[] tempStrings;
    private string[] tempValueStrings;
    
    private void OnEnable()
    {
        TempSetStrings();
        
        // [메모] 피벗이랑 모양을 좀 맞춰야 할 거 같고.... 그냥 다 별롬ㄴ읾ㄴㅇㄹㄴㅁㅇㅇ니ㅓㅏㄹ
        // income의 value만 좀 건들까 그냥?
        // 아님 그냥 한 줄 씩 나오가ㅔㄴㅇㅁㄹ/ㄴㅇ
        // 그리고 duration 적용이 뭔가 이상혀 easing을 줘서 그런가?
        // 다 끝나고 조금 텀이 있다가 작동하는 느낌이야
        
        Sequence sq = DOTween.Sequence();
        
        for (int i = 0; i < 4; i++)
        {
            sq.Append(tempTexts[i].DOText(tempStrings[i], 1f).From("").SetEase(Ease.OutExpo));
            sq.Append(tempValueTexts[i].DOText(tempValueStrings[i], tempValueStrings[i].Length * 0.25f,false
                ,ScrambleMode.Numerals).From("").SetEase(Ease.OutExpo));
            sq.Append(tempValueTextRects[i].DOPunchScale(0.7f*Vector3.one,0.5f).From(true));
        }

        sq.SetUpdate(true);

        SubscribeEvents();
    }

    private void OnDisable()
    {
        UnsubscribeEvents();
    }

    private void TempSetStrings()
    {
        for (int i = 0; i < 4; i++)
        {
            tempTexts[i].text = String.Empty;
            tempValueTexts[i].text = String.Empty;
        }
        
        tempStrings = new string[4];
        tempValueStrings = new string[4];
        
        tempStrings[0] = "Score";
        tempStrings[1] = "Max Combo";
        tempStrings[2] = "Order Delivered";
        tempStrings[3] = "Income";
        
        tempValueStrings[0] = _stageResult.Score.ToString();
        tempValueStrings[1] = _stageResult.MaxCombo.ToString();
        tempValueStrings[2] = _stageResult.DeliveredOrder.ToString();
        int income = _stageResult.CalculateIncome();
        tempValueStrings[3] = $"$ {income / 100}.{income % 100}";
    }

    public void Init(InStageManager sm)
    {
        _stageResult = sm.StageResult;
    }

    private void SubscribeEvents()
    {
        retryBtn.OnClicked += OnRetryClicked;
        lobbyBtn.OnClicked += OnLobbyClicked;
    }

    private void UnsubscribeEvents()
    {
        retryBtn.OnClicked -= OnRetryClicked;
        lobbyBtn.OnClicked -= OnLobbyClicked;
    }

    public void Activate()
    {
        gameObject.SetActive(true);
        // SetTexts();
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

    private void OnRetryClicked()
    {
        GameManager.Instance.ChangeScene("InStage");
        Deactivate();
    }
    
    private void OnLobbyClicked()
    {
        GameManager.Instance.ChangeScene("OutStage");
        Deactivate();
    }
}

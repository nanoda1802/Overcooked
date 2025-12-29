using System;
using DG.Tweening;
using Sfx;
using UnityEngine;
using UnityEngine.UI;
using SF = UnityEngine.SerializeField;

public class StageResult : MonoBehaviour
{
    /* Components */
    [SF] private Transform result;
    /* UI Elements */
    [Header("[ Buttons ]")]
    [SF] private CustomButton lobbyBtn;
    [SF] private CustomButton retryBtn;
    /* SFX */
    [Header("[ SFX ]")]
    [SF] private ClipInfo activateItemSfx; // [임시] 클립 바꿔야함
    [SF] private ClipInfo fanfareSfx; // [임시] 클립 바꿔야함
    /* Fields */
    private StageResultData _stageResult;
    private RectTransform[] _resultItems;
    private Text[] _valueTexts;

    #region Unity Event Methods
    private void OnEnable()
    {
        // score 나오고, maxcombo 나오고, orderdelivered나오고, income나오면서 숫자는 카운터,
        // 카운터 끝나면 punchScale하면서 팡파레 양쪽에서 근데 overlay라서 팡파레가 가려지는디
        Sequence activeSeq = DOTween.Sequence();

        for (int i = 0; i < _resultItems.Length; i++)
        {
            RectTransform curItem = _resultItems[i];
            activeSeq.AppendCallback(() => curItem.gameObject.SetActive(true))
                .JoinCallback(() =>
                    GameManager.Instance.SoundManager.BuildSfx().WithSfxInfo(activateItemSfx).WithRandomPitch().Play());
            
            if (i == _resultItems.Length - 1)
            {
                Text curValueTxt = _valueTexts[i];
                int incomeValue = 0;
                int incomeResult = _stageResult.CalculateIncome();

                activeSeq.Append(DOTween.To(() => incomeValue, x => incomeValue = x, incomeResult, 1.8f)
                        .OnUpdate(() => curValueTxt.text = $"$ {incomeValue / 100}.{incomeValue % 100:D2}").SetEase(Ease.OutExpo))
                    .AppendInterval(0.2f)
                    .Append(curItem.DOScale(1.1f, 0.2f).SetEase(Ease.OutBack))
                    .JoinCallback(() => GameManager.Instance.SoundManager.BuildSfx().WithSfxInfo(fanfareSfx).Play())
                    .Append(curItem.DOScale(1f, 0.5f).SetEase(Ease.InBack));
            }
            else
            {
                activeSeq.Append(curItem.DOScale(1, 0.7f).From(1.02f).SetEase(Ease.OutBack))
                    .AppendInterval(0.3f);
            }
        }
        
        // 그리고 OnComplete로 팡파레 appendCallback
        activeSeq.SetUpdate(true);
        
        SubscribeEvents();
    }

    private void OnDisable()
    {
        UnsubscribeEvents();
    }
    #endregion

    #region Initialize Methods
    public void Init(InStageManager sm)
    {
        _stageResult = sm.StageResult;

        int itemCnt = result.childCount;
        _resultItems = new RectTransform[itemCnt];
        _valueTexts = new Text[itemCnt];

        GetResultItems(itemCnt);
        GetValueTexts(itemCnt);
    }

    private void GetResultItems(int itemCnt)
    {
        if (result is null) return;
        
        for (int i = 0; i < itemCnt; i++)
        {
            _resultItems[i] = result.GetChild(i)?.GetComponent<RectTransform>();
            _resultItems[i].gameObject.SetActive(false);
        }
    }

    private void GetValueTexts(int textCnt)
    {
        if (result is null) return;

        for (int i = 0; i < textCnt; i++)
        {
            _valueTexts[i] = _resultItems[i]?.GetChild(1)?.GetComponent<Text>();
        }
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
    #endregion

    #region UI Control Methods
    public void Activate()
    {
        SetTexts();
        gameObject.SetActive(true);
        // [sfx] 결산창 소리
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }
    
    private void SetTexts() // [임시] 고쳐야해... 인덱스 그대로 넣는 거..
    {
        _valueTexts[0].text = _stageResult.Score.ToString();
        _valueTexts[1].text = _stageResult.MaxCombo.ToString();
        _valueTexts[2].text = $"{_stageResult.DeliveredOrder} ({_stageResult.CalculateDeliverRate()*100:F0}%)";
    }
    #endregion

    #region UI Event Methods
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
    #endregion
}

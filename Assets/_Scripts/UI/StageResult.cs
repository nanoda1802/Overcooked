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
    [SF] private SfxInfo fanfareSfx;
    [SF] private SfxInfo coinSfx;
    /* Fields */
    private StageResultData _stageResult;
    private RectTransform[] _resultItems;
    private Text[] _valueTexts;

    #region Unity Event Methods
    private void OnEnable() // [임시] 고쳐야해....... 읽기 너무 힘들어.......
    {
        Sequence activeSeq = DOTween.Sequence();
        activeSeq.AppendCallback(() =>
            GameManager.Instance.SoundManager.BuildSfx().WithSfxInfo(fanfareSfx).WithRandomPitch().Play());
        
        for (int i = 0; i < _resultItems.Length; i++)
        {
            RectTransform curItem = _resultItems[i];
            activeSeq.AppendCallback(() => curItem.gameObject.SetActive(true));
            
            if (i == _resultItems.Length - 1)
            {
                int incomeResult = _stageResult.CalculateIncome();
                if (incomeResult <= 0) continue;
                
                Text curValueTxt = _valueTexts[i];
                int incomeValue = 0;

                activeSeq.Append(DOTween.To(() => incomeValue, x => incomeValue = x, incomeResult, 1.2f)
                        .OnUpdate(() => curValueTxt.text = $"$ {incomeValue / 100}.{incomeValue % 100:D2}").SetEase(Ease.OutExpo))
                    .AppendInterval(0.1f)
                    .Append(curItem.DOScale(1.1f, 0.2f).SetEase(Ease.OutBack))
                    .JoinCallback(() => GameManager.Instance.SoundManager.BuildSfx().WithSfxInfo(coinSfx).Play())
                    .Append(curItem.DOScale(1f, 0.5f).SetEase(Ease.InBack));
            }
            else
            {
                activeSeq.Append(curItem.DOScale(1, 0.7f).From(1.02f).SetEase(Ease.OutBack))
                    .AppendInterval(0.1f);
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
    public void Init(StageManager sm)
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
        _valueTexts[3].text = "ZERO"; // 여기도 문자열 캐싱해두고
    }
    #endregion

    #region UI Event Methods
    private void OnRetryClicked()
    {
        GameManager.Instance.ChangeScene("Stage");
        Deactivate();
    }
    
    private void OnLobbyClicked()
    {
        GameManager.Instance.ChangeScene("Lobby");
        Deactivate();
    }
    #endregion
}

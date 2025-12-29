using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using SF = UnityEngine.SerializeField;

public class StageSelectPopUp : PopUpUI
{
    [SF] private CustomButton enterBtn;
    [SF] private CustomButton closeBtn;
    
    [SF] private Image stageImg;
    [SF] private Text stageTitle;
    [SF] private Text stageDesc;
    [SF] private Image[] scoreCutImages;
    [SF] private Text[] scoreCutTexts;
    
    [SF] private Color32 achievedTextColor;
    [SF] private Color32 failedTextColor;
    
    [SF] private Toggle dontShowTutorialToggle;
    
    private StageInfoData _stageInfo;

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    public void Activate(OutStagePlayerController player, StageInfoData stageInfo)
    {
        if (IsPopping()) return;
        SubscribeEvents(player);
        SetDisplayInfos(stageInfo);
        gameObject.SetActive(true);
    }

    public void Deactivate(OutStagePlayerController player)
    {
        if (IsPopping()) return;
        UnsubscribeEvents(player);
        Pop(0,popUpTweenTargetPosY,Ease.InBack,1f,()=>gameObject.SetActive(false));
    }

    private void SubscribeEvents(OutStagePlayerController player)
    {
        enterBtn.OnClicked += EnterStage;
        closeBtn.OnClicked += player.DeselectEatery;
        OnBgClicked += player.DeselectEatery;
        dontShowTutorialToggle.onValueChanged.AddListener(OnToggleChanged);
    }

    private void UnsubscribeEvents(OutStagePlayerController player)
    {
        enterBtn.OnClicked -= EnterStage;
        closeBtn.OnClicked -= player.DeselectEatery;
        OnBgClicked -= player.DeselectEatery;
        dontShowTutorialToggle.onValueChanged.RemoveAllListeners();
    }

    private void SetDisplayInfos(StageInfoData stageInfo)
    {
        stageImg.sprite = stageInfo.StageImage;
        stageTitle.text = stageInfo.StageName;
        stageDesc.text = stageInfo.StageDescription;

        int scoreCutIdx = stageInfo.CalculateAchievedScoreCutIndex();
        for (int i = 0; i < 3; i++)
        {
            scoreCutTexts[i].text = $"{stageInfo.ScoreCuts[i]}";
            scoreCutTexts[i].color = scoreCutIdx >= i ? achievedTextColor : failedTextColor;
            scoreCutImages[i].gameObject.SetActive(scoreCutIdx >= i);
        }

        dontShowTutorialToggle.isOn = !stageInfo.ShowTutorial;
        _stageInfo = stageInfo;
    }
    
    private void EnterStage() // [임시] 이게 여깄으면 안 되지 적어도 player나, manager 단에 있어야해
    {
        if (_stageInfo.StageId <= 0) return; // [임시]
        GameManager.Instance.InputManager.ExitOutStage();
        GameManager.Instance.ChangeScene("InStage");
    }

    private void OnToggleChanged(bool value)
    {
        _stageInfo?.SetShowTutorial(!value);
    }
}

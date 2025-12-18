using UnityEngine;
using UnityEngine.UI;
using SF = UnityEngine.SerializeField;

public class StageSelectPopUp : MonoBehaviour
{
    [SF] private Button enterBtn;
    [SF] private Button closeBtn;
    [SF] private Image stageImg;
    [SF] private Text stageTitle;
    [SF] private Text stageDesc;
    [SF] private Image[] scoreCutImages;
    [SF] private Text[] scoreCutTexts;
    
    [SF] private Sprite achievedScoreCutSprite;
    [SF] private Sprite failedScoreCutSprite;
    [SF] private Color32 achievedTextColor;
    [SF] private Color32 failedTextColor;
    
    [SF] private Toggle dontShowTutorialToggle;
    
    [SF] private AudioClip stageEnterSoundClip;
    
    private StageInfoData _stageInfo;
    
    public void SubscribeButtonEvents(OutStagePlayerController player, int stageId)
    {
        enterBtn.onClick.AddListener(()=>
        {
            player.EnterStage(stageId);
            GameManager.Instance.SoundManager.PlaySfx(stageEnterSoundClip);
        });
        closeBtn.onClick.AddListener(()=>
        {
            player.DeselectEatery();
            // [sfx] 버튼 기본 소리
            GameManager.Instance.SoundManager.PlaySfx();
        });
        dontShowTutorialToggle.onValueChanged.AddListener(OnToggleChanged);
    }

    public void UnsubscribeButtonEvents()
    {
        enterBtn.onClick.RemoveAllListeners();
        closeBtn.onClick.RemoveAllListeners();
        dontShowTutorialToggle.onValueChanged.RemoveAllListeners();
    }

    public void SetPopUpInfos(StageInfoData stageInfo)
    {
        stageImg.sprite = stageInfo.StageImage;
        stageTitle.text = stageInfo.StageName;
        stageDesc.text = stageInfo.StageDescription;

        int scoreCutIdx = stageInfo.CalculateAchievedScoreCutIndex();
        for (int i = 0; i < 3; i++)
        {
            scoreCutTexts[i].text = $"{stageInfo.ScoreCuts[i]}";
            scoreCutImages[i].sprite = scoreCutIdx >= i ? achievedScoreCutSprite : failedScoreCutSprite;
            scoreCutTexts[i].color = scoreCutIdx >= i ? achievedTextColor : failedTextColor;
        }

        dontShowTutorialToggle.isOn = !stageInfo.ShowTutorial;
        _stageInfo = stageInfo;
    }
    
    public void OnToggleChanged(bool value)
    {
        _stageInfo.SetShowTutorial(!value);
    }
}

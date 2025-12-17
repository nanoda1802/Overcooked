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
    
    [SF] private AudioClip stageEnterSoundClip;
    
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
    }

    public void UnsubscribeButtonEvents()
    {
        enterBtn.onClick.RemoveAllListeners();
        closeBtn.onClick.RemoveAllListeners();
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
    }
}

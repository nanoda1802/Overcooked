using System.Collections;
using System.Collections.Generic;
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
    
    public void SubscribeButtonEvents(OutStagePlayerController player, int stageId)
    {
        enterBtn.onClick.AddListener(()=>player.EnterStage(stageId));
        closeBtn.onClick.AddListener(player.DeselectEatery);
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

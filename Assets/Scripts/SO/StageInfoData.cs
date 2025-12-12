using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SF = UnityEngine.SerializeField;

[CreateAssetMenu(fileName = "StageInfoData", menuName = "SO/Stage/StageInfo")]
public class StageInfoData : ScriptableObject
{
    [SF] private int stageId;
    [SF] private string stageName;
    [SF] private string stageDescription;
    [SF] private Sprite stageImage;
    [SF] private int bestScore; // [임시] 다른 곳에 저장해야 해
    [SF] private int[] scoreCuts;
    [SF] private float stageDuration;
    [SF] private Grid[] gridInfos; // [임시]
    [SF] private OrderInfoData orderInfoData;
    
    public int StageId => stageId;
    public string StageName => stageName;
    public string StageDescription => stageDescription;
    public Sprite StageImage => stageImage;
    public int BestScore => bestScore;
    public int[] ScoreCuts => scoreCuts;
    public float StageDuration => stageDuration;
    public Grid[] GridInfos => gridInfos;
    public OrderInfoData OrderInfoData => orderInfoData;

    public int CalculateAchievedScoreCutIndex() // [임시] 매개변수로 최고 점수 받아야해
    {
        int idx = -1;
        for (int i = 0; i < scoreCuts.Length; i++)
        {
            if (scoreCuts[i] > bestScore) break;
            idx = i;
        }
        return idx;
    }
}

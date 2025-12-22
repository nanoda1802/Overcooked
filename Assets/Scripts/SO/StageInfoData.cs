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
    [SF] private Vector3[] respawnPoints;
    [SF] private OrderInfoData orderInfoData;
    
    [SF] private AudioClip bgm;
    [SF] private bool showTutorial = true;
    
    public int StageId => stageId;
    public string StageName => stageName;
    public string StageDescription => stageDescription;
    public Sprite StageImage => stageImage;
    public int BestScore => bestScore;
    public int[] ScoreCuts => scoreCuts;
    public float StageDuration => stageDuration;
    public Grid[] GridInfos => gridInfos;
    public OrderInfoData OrderInfoData => orderInfoData;
    public AudioClip Bgm => bgm;
    public bool ShowTutorial => showTutorial;
    
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

    public Vector3 GetClosestRespawnPoint(Vector3 pos)
    {
        float minDist = float.MaxValue;
        Vector3 closest = Vector3.one;
        
        foreach (Vector3 point in respawnPoints)
        {
            float dist = (point - pos).sqrMagnitude;
            Debug.Log($"이번 지점 {point}, 거리 {dist}");
            if (dist >= minDist) continue;
            
            minDist = dist;
            closest = point;
        }

        return closest;
    }

    public void SetShowTutorial(bool value)
    {
        showTutorial = value;
    }
}

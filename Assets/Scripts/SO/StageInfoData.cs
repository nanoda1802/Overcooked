using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SF = UnityEngine.SerializeField;

[CreateAssetMenu(fileName = "StageInfoData", menuName = "SO/Stage/StageInfo")]
public class StageInfoData : ScriptableObject
{
    [SF] private string stageName; // [임시]
    [SF] private float stageDuration;
    [SF] private Grid[] gridInfos; // [임시]
    [SF] private OrderInfoData orderInfoData;
    
    public string StageName => stageName;
    public float StageDuration => stageDuration;
    public Grid[] GridInfos => gridInfos;
    public OrderInfoData OrderInfoData => orderInfoData;
}

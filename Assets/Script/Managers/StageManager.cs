using System;
using UnityEngine;
using SF =  UnityEngine.SerializeField;

public class StageManager : MonoBehaviour
{
    [SF] private StageInfoData stageInfo;
    
    // 하위 매니저들에 대한 참조
    [SF] private ScoreManager scoreManager;
    [SF] private OrderManager orderManager;
    [SF] private PoolManager poolManager;
    
    private void Awake()
    {
        scoreManager.Init();
        orderManager.Init(scoreManager, stageInfo.OrderInfoData);
        // poolManager.Init();
        
        Application.targetFrameRate = 60; // [임시]
    }
}

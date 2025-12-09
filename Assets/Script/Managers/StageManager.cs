using UnityEngine;
using SF =  UnityEngine.SerializeField;

public class StageManager : MonoBehaviour
{
    [SF] private StageInfoData stageInfo;
    public StageInfoData StageInfo => stageInfo;
    
    [SF] private GameObject stageClearUI;
    
    // 하위 매니저들에 대한 참조
    [SF] private ScoreManager scoreManager;
    [SF] private OrderManager orderManager;
    [SF] private PoolManager poolManager;
    [SF] private TimeManager timeManager;
    
    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        Application.targetFrameRate = 60; // [임시]
        
        scoreManager.Init();
        orderManager.Init(scoreManager, stageInfo.OrderInfoData);
        timeManager.Init(this);
        // poolManager.Init();
    }

    public void FinishStage() // [임시]
    {
        // 모든 작동을 중지시켜야 해
        // om은 모든 주문들 타이머 멈추고, 새 주문 생성 멈추고
        // tm도 시간 더 안 가게 멈추고
        // 캐릭터도 못 움직이게 하고
        // 등등
        scoreManager.gameObject.SetActive(false);
        orderManager.gameObject.SetActive(false);
        timeManager.gameObject.SetActive(false);
        // 이걸로는 부족함
        
        stageClearUI.SetActive(true);
    }
}

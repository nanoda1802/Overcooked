using UnityEngine;
using SF =  UnityEngine.SerializeField;

public interface IManager
{
    public void Init(StageManager sm);
    public void Deinit();
}

public class StageManager : MonoBehaviour
{
    [SF] private StageInfoData stageInfo;
    [SF] private StageResultData stageResult;
    public StageInfoData StageInfo => stageInfo;
    public StageResultData StageResult => stageResult;
    
    [SF] private StageResult stageResultUI;
    
    // 하위 매니저들에 대한 참조
    [SF] private ScoreManager scoreManager;
    [SF] private OrderManager orderManager;
    [SF] private PoolManager poolManager;
    [SF] private TimeManager timeManager;
    public ScoreManager ScoreManager => scoreManager;
    
    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        Application.targetFrameRate = 60; // [임시]
        
        scoreManager.Init(this);
        orderManager.Init(this);
        timeManager.Init(this);
        // poolManager.Init();
        
        stageResultUI.Init(this);
    }

    public void FinishStage() // [임시]
    {
        // 모든 작동을 중지시켜야 해
        // om은 모든 주문들 타이머 멈추고, 새 주문 생성 멈추고
        // tm도 시간 더 안 가게 멈추고
        // 캐릭터도 못 움직이게 하고 -> 이거 어떻게 해야할지........
        // 등등
        scoreManager.Deinit();
        orderManager.Deinit();
        timeManager.Deinit();
        // 이걸로는 부족함
        
        stageResultUI.Activate();
    }
}

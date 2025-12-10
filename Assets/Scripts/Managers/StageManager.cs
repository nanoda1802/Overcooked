using UnityEngine;
using SF =  UnityEngine.SerializeField;

public interface IManager
{
    public void Init(StageManager sm);
    public void Deinit();
}

public enum StageState
{
    Paused,
    Running,
    Finished
}

public class StageManager : MonoBehaviour
{
    [SF] private StageInfoData stageInfo;
    [SF] private StageResultData stageResult;
    public StageInfoData StageInfo => stageInfo;
    public StageResultData StageResult => stageResult;
    
    [SF] private StageResult stageResultUI;

    [SF] private PausePanel pauseUI;
    [SF] private bool isStagePaused;
    public bool IsStagePaused => isStagePaused;
    
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
        ResumeStage();
        
        scoreManager.Init(this);
        orderManager.Init(this);
        timeManager.Init(this);
        // poolManager.Init();
        
        stageResultUI.Init(this);
        pauseUI.Init(this);
    }

    public void PauseStage()
    {
        // inputManager.SetEnableGlobalActionMap()
        // 적절한 액션에 맞는 메서드 구독
        timeManager.PauseTime();
        isStagePaused = true;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        pauseUI.Activate();
    }

    public void ResumeStage()
    {
        // inputManager.SetDisableGlobalActionMap()
        // 등록해둔 메서드 구독해제
        timeManager.ResumeTime();
        isStagePaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        pauseUI.Deactivate();
    }

    public void FinishStage() // [임시]
    {
        PauseStage(); // [임시]... 일단 결과창이 덮으니까 괜찮긴 한데 이게...
        stageResultUI.Activate();
    }
}

using UnityEngine;
using SF =  UnityEngine.SerializeField;

public interface IManager
{
    public void Init(InStageManager sm);
    public void Deinit();
}

public enum StageState
{
    Paused,
    Running,
    Finished
}

public class InStageManager : MonoBehaviour
{
    [SF] private GameManager gameManager;
    public GameManager GameManager => gameManager;
    
    [SF] private StageInfoData stageInfo;
    [SF] private StageResultData stageResult;
    public StageInfoData StageInfo => stageInfo;
    public StageResultData StageResult => stageResult;
    
    [SF] private StageResult stageResultUI;
    [SF] private TutorialPopUp tutorialUI;
    
    [SF] private PausePopUp pauseUI;
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
        
        gameManager ??= FindObjectOfType(typeof(GameManager)) as GameManager;
        GameManager.Instance.SoundManager.ChangeBgm(stageInfo.Bgm);
        
        scoreManager.Init(this);
        orderManager.Init(this);
        timeManager.Init(this);
        // poolManager.Init();
        
        stageResultUI.Init(this);
        pauseUI.Init(this);
        tutorialUI.Init(this);
        
        if (stageInfo.ShowTutorial)
        {
            PauseStage();
            tutorialUI.Activate();
        }
        else
        {
            ResumeStage();
            gameManager?.InputManager.EnterInStage();
        }
    }

    public void PauseStage(bool withPopUp = false)
    {
        timeManager.PauseTimer();
        isStagePaused = true;
        
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        
        if (withPopUp) pauseUI.Activate();
    }

    public void ResumeStage()
    {
        timeManager.ResumeTimer();
        isStagePaused = false;
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        if (pauseUI.IsActive) pauseUI.Deactivate();
    }

    public void RetryStage() // [임시] 씬 전환 없이 할 방법 생각해보기
    {
        if (pauseUI.IsActive) pauseUI.Deactivate();
        gameManager.SoundManager.TurnOffAllSfx();
        gameManager.SoundManager.TurnOffCurrentBgm(true);
        gameManager.InputManager.ExitInStage();
        gameManager.ChangeScene("InStage");
    }

    public void QuitStage()
    {
        if (pauseUI.IsActive) pauseUI.Deactivate();
        gameManager.SoundManager.TurnOffAllSfx();
        gameManager.SoundManager.TurnOffCurrentBgm(true);
        FinishStage(); 
    }

    public void FinishStage() // [임시]
    {
        gameManager.InputManager.ExitInStage();
        gameManager.SoundManager.TurnOffCurrentBgm(); // [임시]
        gameManager.SoundManager.TurnOffAllSfx(); // [임시]
        timeManager.Deinit(); // [임시]
        orderManager.Deinit(); // [임시]
        scoreManager.Deinit(); // [임시]
        PauseStage();
        stageResultUI.Activate();
    }
}

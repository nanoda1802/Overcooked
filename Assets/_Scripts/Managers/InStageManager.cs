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

    [SF] private StageCue stageCueUI;
    [SF] private StageResult stageResultUI;
    [SF] private TutorialPopUp tutorialUI;
    [SF] private PausePopUp pauseUI;
    public StageCue StageCueUI => stageCueUI; // [임시]
    public StageResult StageResultUI => stageResultUI; // [임시]
    public PausePopUp PauseUI => pauseUI; // [임시]
    
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
        
        stageCueUI.Init(this);
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
            stageCueUI.Activate(CueType.Start);
        }
    }

    public void StartStage()
    {
        ResumeStage();
        GameManager.Instance.InputManager.EnterInStage();
    }

    public void PauseStage()
    {
        timeManager.PauseTimer();
        
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    public void ResumeStage()
    {
        timeManager.ResumeTimer();
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void RetryStage() // [임시] 씬 전환 없이 할 방법 생각해보기
    {
        // if (pauseUI.IsActive) pauseUI.Deactivate();
        gameManager.InputManager.ExitInStage();
        gameManager.SoundManager.TurnOffCurrentBgm(true);
        gameManager.SoundManager.TurnOffActiveSfx();
        gameManager.ChangeScene("InStage");
    }

    public void QuitStage() // [임시] 이 retry quit end 삼형제 어케... 개선해봐... 
    {
        // if (pauseUI.IsActive) pauseUI.Deactivate();
        gameManager.InputManager.ExitInStage();
        gameManager.SoundManager.TurnOffCurrentBgm(true); // [임시]
        gameManager.SoundManager.TurnOffActiveSfx(); // [임시]
        timeManager.Deinit(); // [임시]
        orderManager.Deinit(); // [임시]
        scoreManager.Deinit(); // [임시]
        stageCueUI.Activate(CueType.Quit);
    }

    public void EndStage() // [임시]
    {
        gameManager.InputManager.ExitInStage();
        gameManager.SoundManager.TurnOffCurrentBgm(); // [임시]
        gameManager.SoundManager.TurnOffActiveSfx(); // [임시]
        timeManager.Deinit(); // [임시]
        orderManager.Deinit(); // [임시]
        scoreManager.Deinit(); // [임시]
        PauseStage();
        
        stageCueUI.Activate(CueType.Timeout);
        // stageResultUI.Activate();
    }
}

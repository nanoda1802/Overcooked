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
    [SF] private bool isStagePaused;
    public StageCue StageCueUI => stageCueUI; // [임시]
    public StageResult StageResultUI => stageResultUI; // [임시]
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

    public void QuitStage() // [임시] 이 retry quit end 삼형제 어케... 개선해봐... 
    {
        if (pauseUI.IsActive) pauseUI.Deactivate();
        gameManager.InputManager.ExitInStage();
        gameManager.SoundManager.TurnOffCurrentBgm(true); // [임시]
        gameManager.SoundManager.TurnOffAllSfx(); // [임시]
        timeManager.Deinit(); // [임시]
        orderManager.Deinit(); // [임시]
        scoreManager.Deinit(); // [임시]
        stageCueUI.Activate(CueType.Quit);
    }

    public void EndStage() // [임시]
    {
        gameManager.InputManager.ExitInStage();
        gameManager.SoundManager.TurnOffCurrentBgm(); // [임시]
        gameManager.SoundManager.TurnOffAllSfx(); // [임시]
        timeManager.Deinit(); // [임시]
        orderManager.Deinit(); // [임시]
        scoreManager.Deinit(); // [임시]
        PauseStage();
        
        stageCueUI.Activate(CueType.End);
        // stageResultUI.Activate();
    }
}

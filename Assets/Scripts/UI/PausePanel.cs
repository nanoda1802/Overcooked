using UnityEngine;

public class PausePanel : MonoBehaviour
{
    private InStageManager _inStageManager;
    private GameManager _gameManager;

    public void Init(InStageManager sm)
    {
        _inStageManager = sm;
        _gameManager = sm.GameManager;
    }

    public void Activate()
    {
        gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }

    public void OnResume()
    {
        _inStageManager.ResumeStage();
    }

    public void OnRetry()
    {
        _gameManager.InputManager.ExitInStage();
        _gameManager.ChangeScene("InStage");
        Deactivate();
    }

    public void OnQuit()
    {
        _inStageManager.FinishStage();
        Deactivate();
    }
}

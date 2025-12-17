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
        // [sfx] 퍼즈될 때 소리
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }

    public void OnResume()
    {
        _inStageManager.ResumeStage();
        // [sfx] 버튼 기본 소리
        GameManager.Instance.SoundManager.PlaySfx();
    }

    public void OnRetry()
    {
        _gameManager.InputManager.ExitInStage();
        _gameManager.ChangeScene("InStage");
        Deactivate();
        // [sfx] 버튼 기본 소리
        GameManager.Instance.SoundManager.PlaySfx();
    }

    public void OnQuit()
    {
        _inStageManager.FinishStage();
        Deactivate();
        // [sfx] 버튼 기본 소리
        GameManager.Instance.SoundManager.PlaySfx();
    }
}

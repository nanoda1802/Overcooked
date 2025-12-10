using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PausePanel : MonoBehaviour
{
    private StageManager _stageManager;

    public void Init(StageManager sm)
    {
        _stageManager = sm;
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
        _stageManager.ResumeStage();
    }

    public void OnRetry()
    {
        SceneManager.LoadScene(0); // [임시]
    }

    public void OnQuit()
    {
        _stageManager.FinishStage();
    }
}

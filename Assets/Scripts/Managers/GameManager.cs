using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using SF = UnityEngine.SerializeField;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance => _instance;
    
    [SF] private InputManager inputManager;
    public InputManager InputManager => inputManager;
    
    [SF] private SoundManager soundManager;
    public SoundManager SoundManager => soundManager;
    
    [SF] private LoadingDisplay loadingDisplay;
    
    private void Awake()
    {
        if (_instance is null)
        {
            _instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        inputManager.EnterOutStage();
        soundManager.InitPool();
    }

    public void ChangeScene(string sceneName)
    {
        soundManager.MuteCurrentBgm();
        StartCoroutine(CoLoadSceneAsync(sceneName));
    }

    private IEnumerator CoLoadSceneAsync(string sceneName)
    {
        AsyncOperation loadOper = SceneManager.LoadSceneAsync(sceneName);
        if (loadOper is null) yield break;
        
        loadOper.allowSceneActivation = false;
        Time.timeScale = 0f;
        loadingDisplay.Activate();

        float minTime = 0.3f;
        
        while (!loadOper.isDone)
        {
            if (loadOper.progress < 0.9f)
            {
                loadingDisplay.UpdateProgressImage(Mathf.Lerp(0f,0.3f,loadOper.progress * 0.3f));
                yield return null;
            }
            else
            {
                minTime += Time.unscaledDeltaTime;
                loadingDisplay.UpdateProgressImage(Mathf.Lerp(0.3f,1f,minTime));
                yield return null;
                if (minTime < 1f) continue;
            
                loadOper.allowSceneActivation = true;
                yield return StartCoroutine(loadingDisplay.Fade(1, 0));
                Time.timeScale = 1f;
                yield break;
            }
        }
    }
}

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
    }

    public IEnumerator CoLoadSceneAsync(string sceneName)
    {
        AsyncOperation loadOper = SceneManager.LoadSceneAsync(sceneName);
        Time.timeScale = 1f;

        while (!loadOper.isDone)
        {
            yield return null;
            if (loadOper.progress < 0.9f) continue;
            
            loadOper.allowSceneActivation = true;
            yield return null;
            Time.timeScale = 1f;
            yield break;
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
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
        Debug.Log($"전환 호출 ? {loadOper is not null}");
        Time.timeScale = 1f;

        while (!loadOper.isDone)
        {
            yield return null;

            Debug.Log(loadOper.progress);
            if (loadOper.progress < 0.9f) continue;
            loadOper.allowSceneActivation = true;
            Time.timeScale = 1f;
            Debug.Log("씬 전환");
            yield break;
        }

        yield return null;
    }
}

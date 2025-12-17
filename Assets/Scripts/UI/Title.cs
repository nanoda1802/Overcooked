using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using SF = UnityEngine.SerializeField;

public class Title : MonoBehaviour
{
    [SF] private CinemachineVirtualCamera titleCam;
    [SF] private GameObject titleUI;
    [SF] private GameObject settingsUI;

    
    public void OnSelectStage()
    {
        titleUI.SetActive(false);
        titleCam.Priority = 0;
        // [sfx] 버튼 기본 소리
        GameManager.Instance.SoundManager.PlaySfx();
    }

    public void OnSettings()
    {
        settingsUI.SetActive(true);
        // [sfx] 버튼 기본 소리
        GameManager.Instance.SoundManager.PlaySfx();
    }

    public void OnExit()
    {
        Application.Quit();
        // [sfx] 버튼 기본 소리
        GameManager.Instance.SoundManager.PlaySfx();
    }
}

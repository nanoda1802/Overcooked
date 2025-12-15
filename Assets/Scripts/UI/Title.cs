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
    }

    public void OnSettings()
    {
        settingsUI.SetActive(true);
    }

    public void OnExit()
    {
        Application.Quit();
    }
}

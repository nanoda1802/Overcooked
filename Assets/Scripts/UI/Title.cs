using Cinemachine;
using UnityEngine;
using SF = UnityEngine.SerializeField;

public class Title : MonoBehaviour
{
    [SF] private CinemachineVirtualCamera titleCam;
    [SF] private SettingsPopUp settingsUI;

    [SF] private CustomButton btnSelectStage;
    [SF] private CustomButton btnSettings;
    [SF] private CustomButton btnExit;

    private void OnEnable()
    {
        btnSelectStage.SubscribeEvent(OnSelectStage);
        btnSettings.SubscribeEvent(OnSettings);
        btnExit.SubscribeEvent(OnExit);
    }

    private void OnDisable()
    {
        btnSelectStage.UnsubscribeEvent(OnSelectStage);
        btnSettings.UnsubscribeEvent(OnSettings);
        btnExit.UnsubscribeEvent(OnExit);
    }

    public void OnSelectStage()
    {
        gameObject.SetActive(false);
        GameManager.Instance.InputManager.EnterOutStage();
        titleCam.Priority = 0;
    }

    public void OnSettings()
    {
        settingsUI.gameObject.SetActive(true);
    }

    public void OnExit()
    {
        // Application.Quit();
    }
}

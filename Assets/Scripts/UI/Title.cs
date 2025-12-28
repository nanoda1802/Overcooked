using Cinemachine;
using UnityEngine;
using SF = UnityEngine.SerializeField;

public class Title : MonoBehaviour
{
    [SF] private CinemachineVirtualCamera titleCam;
    [SF] private SettingsPopUp settingsUI;

    [SF] private CustomButton selectStageBtn;
    [SF] private CustomButton settingsBtn;
    [SF] private CustomButton exitBtn;

    private void OnEnable()
    {
        // selectStageBtn.SubscribeEvent(OnSelectStage);
        // settingsBtn.SubscribeEvent(OnSettings);
        // exitBtn.SubscribeEvent(OnExit);
        SubscribeEvents();
    }

    private void OnDisable()
    {
        // selectStageBtn.UnsubscribeEvent(OnSelectStage);
        // settingsBtn.UnsubscribeEvent(OnSettings);
        // exitBtn.UnsubscribeEvent(OnExit);
        UnsubscribeEvents();
    }

    private void SubscribeEvents()
    {
        selectStageBtn.OnClicked += OnSelectStageClicked;
        settingsBtn.OnClicked += OnSettingsClicked;
        exitBtn.OnClicked += OnExitClicked;
    }

    private void UnsubscribeEvents()
    {
        selectStageBtn.OnClicked -= OnSelectStageClicked;
        settingsBtn.OnClicked -= OnSettingsClicked;
        exitBtn.OnClicked -= OnExitClicked;
    }

    private void OnSelectStageClicked()
    {
        gameObject.SetActive(false);
        GameManager.Instance.InputManager.EnterOutStage();
        titleCam.Priority = 0;
    }

    private void OnSettingsClicked()
    {
        settingsUI.Activate();
    }

    private void OnExitClicked()
    {
        // Application.Quit();
    }
}

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
        SubscribeEvents();
    }

    private void OnDisable()
    {
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
        // 여기서 플레이어의 Pose를 -1로 바꿔줘야하는디.......
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

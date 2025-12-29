using DG.Tweening;
using UnityEngine.UI;
using SF = UnityEngine.SerializeField;

public class SettingsPopUp : PopUpUI
{
    [SF] private SettingsData settingsInfo;
    
    [SF] private CustomSlider bgmSlider;
    [SF] private CustomSlider sfxSlider;
    [SF] private Toggle bgmMuteToggle;
    [SF] private Toggle sfxMuteToggle;

    [SF] private CustomButton applyBtn;
    [SF] private CustomButton closeBtn;

    protected override void OnEnable()
    {
        ApplyDataToUI();

        SubscribeEvents();
        base.OnEnable();
    }

    protected override void OnDisable()
    {
        UnsubscribeEvents();
        base.OnDisable();
    }

    public void Activate()
    {
        if (IsPopping()) return;
        gameObject.SetActive(true);
    }

    private void Deactivate()
    {
        if (IsPopping()) return;
        Pop(0,popUpTweenTargetPosY,Ease.InBack,1f,()=>gameObject.SetActive(false));
    }

    private void SubscribeEvents()
    {
        bgmSlider.OnValueChanged += settingsInfo.SetBgmVolume;
        sfxSlider.OnValueChanged += settingsInfo.SetSfxVolume;
        applyBtn.OnClicked += OnApplyClicked;
        closeBtn.OnClicked += OnCloseClicked;
        OnBgClicked += OnCloseClicked;
    }

    private void UnsubscribeEvents()
    {
        bgmSlider.OnValueChanged -= settingsInfo.SetBgmVolume;
        sfxSlider.OnValueChanged -= settingsInfo.SetSfxVolume;
        applyBtn.OnClicked -= OnApplyClicked;
        closeBtn.OnClicked -= OnCloseClicked;
        OnBgClicked -= OnCloseClicked;
    }

    private void ApplyDataToUI()
    {
        bgmSlider.SyncSliderElements(settingsInfo.BgmVolume);
        sfxSlider.SyncSliderElements(settingsInfo.SfxVolume);
        
        bgmMuteToggle.isOn = settingsInfo.IsBgmMute;
        sfxMuteToggle.isOn = settingsInfo.IsSfxMute;
    }

    private void OnApplyClicked()
    {
        // 나중에 다른 설정값 갱신... 그래픽이나 프레임 같은 거...
        settingsInfo.SetIsBgmMute(bgmMuteToggle.isOn);
        settingsInfo.SetIsSfxMute(sfxMuteToggle.isOn);
    }
    
    private void OnCloseClicked()
    {
        Deactivate();
    }
}

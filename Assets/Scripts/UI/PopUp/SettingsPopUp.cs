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
        bgmSlider.SubscribeEvent(settingsInfo.SetBgmVolume);
        sfxSlider.SubscribeEvent(settingsInfo.SetSfxVolume);
        applyBtn.SubscribeEvent(OnApplyButton);
        closeBtn.SubscribeEvent(OnCloseButtonClicked);

        OnBgClicked += OnCloseButtonClicked;
        
        base.OnEnable();
    }

    protected override void OnDisable()
    {
        bgmSlider.UnsubscribeEvent(settingsInfo.SetBgmVolume);
        sfxSlider.UnsubscribeEvent(settingsInfo.SetSfxVolume);
        applyBtn.UnsubscribeEvent(OnApplyButton);
        closeBtn.UnsubscribeEvent(OnCloseButtonClicked);
        
        OnBgClicked -= OnCloseButtonClicked;
        
        base.OnDisable();
    }

    private void ApplyDataToUI()
    {
        bgmSlider.SyncSliderElements(settingsInfo.BgmVolume);
        sfxSlider.SyncSliderElements(settingsInfo.SfxVolume);
        
        bgmMuteToggle.isOn = settingsInfo.IsBgmMute;
        sfxMuteToggle.isOn = settingsInfo.IsSfxMute;
    }

    private void OnApplyButton()
    {
        // 나중에 다른 설정값 갱신... 그래픽이나 프레임 같은 거...
        settingsInfo.SetIsBgmMute(bgmMuteToggle.isOn);
        settingsInfo.SetIsSfxMute(sfxMuteToggle.isOn);
    }
    
    private void OnCloseButtonClicked()
    {
        DoMoveYTransition(0,popUpTweenTargetPosY,Ease.InBack,1f,()=>gameObject.SetActive(false));
    }
}

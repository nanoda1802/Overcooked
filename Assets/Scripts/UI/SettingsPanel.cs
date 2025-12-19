using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SF = UnityEngine.SerializeField;

public class SettingsPanel : MonoBehaviour
{
    [SF] private SettingsData settingsInfo;
    
    [SF] private Slider bgmSlider;
    [SF] private Slider sfxSlider;
    [SF] private Toggle bgmMuteToggle;
    [SF] private Toggle sfxMuteToggle;

    [SF] private CustomButton btnApply;
    [SF] private CustomButton btnQuit;
    
    // 바뀐 옵션만 적용되게 해야 겄는디

    private void OnEnable()
    {
        ApplySettingsInfo();
        btnApply.SubscribeEvent(OnApplyButton);
        btnQuit.SubscribeEvent(OnQuitButton);
    }

    private void OnDisable()
    {
        btnApply.UnsubscribeEvent(OnApplyButton);
        btnQuit.UnsubscribeEvent(OnQuitButton);
    }

    private void ApplySettingsInfo()
    {
        bgmSlider.value = settingsInfo.BgmVolume;
        sfxSlider.value = settingsInfo.SfxVolume;
        bgmMuteToggle.isOn = settingsInfo.IsBgmMute;
        sfxMuteToggle.isOn = settingsInfo.IsSfxMute;
    }

    private void OnApplyButton()
    {
        settingsInfo.SetBgmVolume(bgmSlider.value);
        settingsInfo.SetSfxVolume(sfxSlider.value);
        settingsInfo.SetIsBgmMute(bgmMuteToggle.isOn);
        settingsInfo.SetIsSfxMute(sfxMuteToggle.isOn);
    }

    private void OnQuitButton()
    {
        gameObject.SetActive(false);
    }

    public void OnBgmSliderChanged(float value)
    {
        // 이 옵션이 바뀌었소
    }

    public void OnSfxSliderChanged(float value)
    {
        // 이 옵션이 바뀌었소
    }

    public void OnBgmMuteToggleChanged(bool value)
    {
        // 이 옵션이 바뀌었소
    }

    public void OnSfxMuteToggleChanged(bool value)
    {
        // 이 옵션이 바뀌었소
    }
}

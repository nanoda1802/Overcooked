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
    
    // 바뀐 옵션만 적용되게 해야 겄는디

    private void OnEnable()
    {
        bgmSlider.value = settingsInfo.BgmVolume;
        sfxSlider.value = settingsInfo.SfxVolume;
        bgmMuteToggle.isOn = settingsInfo.IsBgmMute;
        sfxMuteToggle.isOn = settingsInfo.IsSfxMute;
    }

    public void OnApplyButton()
    {
        settingsInfo.SetBgmVolume(bgmSlider.value);
        settingsInfo.SetSfxVolume(sfxSlider.value);
        settingsInfo.SetIsBgmMute(bgmMuteToggle.isOn);
        settingsInfo.SetIsSfxMute(sfxMuteToggle.isOn);
        GameManager.Instance.SoundManager.PlaySfx();
    }

    public void OnQuitButton()
    {
        GameManager.Instance.SoundManager.PlaySfx();
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

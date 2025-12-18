using System;
using UnityEngine;
using UnityEngine.UI;
using SF = UnityEngine.SerializeField;

public class PausePanel : MonoBehaviour
{
    private InStageManager _inStageManager;
    private GameManager _gameManager;

    private SettingsData _settingsInfo;
    
    [SF] private Slider bgmSlider;
    [SF] private Slider sfxSlider;
    [SF] private Toggle bgmMuteToggle;
    [SF] private Toggle sfxMuteToggle;

    public void Init(InStageManager sm)
    {
        _inStageManager = sm;
        _gameManager = sm.GameManager;
        _settingsInfo = _gameManager.SettingsData;
    }

    public void Activate()
    {
        gameObject.SetActive(true);
        _gameManager?.SoundManager.PauseAllSounds(true);
        bgmSlider.value = _settingsInfo.BgmVolume;
        sfxSlider.value = _settingsInfo.SfxVolume;
        bgmMuteToggle.isOn = _settingsInfo.IsBgmMute;
        sfxMuteToggle.isOn = _settingsInfo.IsSfxMute;
        // [sfx] 퍼즈될 때 소리
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
        _gameManager?.SoundManager.PauseAllSounds(false);
    }

    public void OnBgmSliderChanged()
    {
        _settingsInfo.SetBgmVolume(bgmSlider.value);
    }

    public void OnSfxSliderChanged()
    {
        _settingsInfo.SetSfxVolume(sfxSlider.value);
    }

    public void OnBgmMuteToggleChanged(bool value)
    {
        _settingsInfo.SetIsBgmMute(value);
    }

    public void OnSfxMuteToggleChanged(bool value)
    {
        _settingsInfo.SetIsSfxMute(value);
    }
    
    public void OnResume()
    {
        _inStageManager.ResumeStage();
        // [sfx] 버튼 기본 소리
        GameManager.Instance.SoundManager.PlaySfx();
    }

    public void OnRetry()
    {
        _gameManager.SoundManager.TurnOffAllSfx();
        _gameManager.SoundManager.TurnOffCurrentBgm(true);
        _gameManager.InputManager.ExitInStage();
        _gameManager.ChangeScene("InStage");
        Deactivate();
        // [sfx] 버튼 기본 소리
        GameManager.Instance.SoundManager.PlaySfx();
    }

    public void OnQuit()
    {
        _gameManager.SoundManager.TurnOffAllSfx();
        _gameManager.SoundManager.TurnOffCurrentBgm(true);
        _inStageManager.FinishStage();
        Deactivate();
        // [sfx] 버튼 기본 소리
        GameManager.Instance.SoundManager.PlaySfx();
    }
}

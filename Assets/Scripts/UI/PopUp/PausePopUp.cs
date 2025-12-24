using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using SF = UnityEngine.SerializeField;

public class PausePopUp : PopUpUI
{
    private InStageManager _inStageManager;
    private SettingsData _settingsInfo;
    
    private bool _isActive;
    public bool IsActive => _isActive;
    
    [SF] private CustomButton resumeBtn;
    [SF] private CustomButton retryBtn;
    [SF] private CustomButton quitBtn;
    
    [SF] private CustomSlider bgmSlider;
    [SF] private CustomSlider sfxSlider;
    
    [SF] private Toggle bgmMuteToggle;
    [SF] private Toggle sfxMuteToggle;

    [SF] private AudioClip pauseSfx;
    [SF] private AudioClip unpauseSfx;
    
    protected override void OnEnable()
    {
        base.OnEnable();
        _isActive = true;
        
        ApplyDataToUI();
        SubscribeEvents();
        
        GameManager.Instance.SoundManager.PauseAllSounds(true);
        GameManager.Instance.SoundManager.PlaySfx(pauseSfx);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        UnsubscribeEvents();
        GameManager.Instance.SoundManager.PauseAllSounds(false);
    }

    public void Init(InStageManager sm)
    {
        _inStageManager = sm;
        _settingsInfo = GameManager.Instance.SettingsData;
    }

    public void Activate()
    {
        gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        if (!_isActive) return;
        _isActive = false;
        DoMoveYTransition(0,popUpTweenTargetPosY,Ease.InBack,1f,()=>gameObject.SetActive(false));
    }

    private void SubscribeEvents()
    {
        resumeBtn.SubscribeEvent(_inStageManager.ResumeStage);
        retryBtn.SubscribeEvent(_inStageManager.RetryStage);
        quitBtn.SubscribeEvent(_inStageManager.QuitStage);
        bgmSlider.SubscribeEvent(_settingsInfo.SetBgmVolume);
        sfxSlider.SubscribeEvent(_settingsInfo.SetSfxVolume);
        OnBgClicked += _inStageManager.ResumeStage;
        
        bgmMuteToggle.onValueChanged.AddListener(OnBgmMuteToggleChanged);
        sfxMuteToggle.onValueChanged.AddListener(OnSfxMuteToggleChanged);
    }

    private void UnsubscribeEvents()
    {
        resumeBtn.UnsubscribeEvent(_inStageManager.ResumeStage);
        retryBtn.UnsubscribeEvent(_inStageManager.RetryStage);
        quitBtn.UnsubscribeEvent(_inStageManager.QuitStage);
        bgmSlider.UnsubscribeEvent(_settingsInfo.SetBgmVolume);
        sfxSlider.UnsubscribeEvent(_settingsInfo.SetSfxVolume);
        OnBgClicked -= _inStageManager.ResumeStage;
        
        bgmMuteToggle.onValueChanged.RemoveAllListeners();
        sfxMuteToggle.onValueChanged.RemoveAllListeners();
    }

    private void ApplyDataToUI()
    {
        bgmSlider.SyncSliderElements(_settingsInfo.BgmVolume);
        sfxSlider.SyncSliderElements(_settingsInfo.SfxVolume);
        
        bgmMuteToggle.isOn = _settingsInfo.IsBgmMute;
        sfxMuteToggle.isOn = _settingsInfo.IsSfxMute;
    }

    private void OnBgmMuteToggleChanged(bool value)
    {
        _settingsInfo.SetIsBgmMute(value);
    }

    private void OnSfxMuteToggleChanged(bool value)
    {
        _settingsInfo.SetIsSfxMute(value);
    }
}

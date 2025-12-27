using DG.Tweening;
using Sfx;
using UnityEngine.UI;
using SF = UnityEngine.SerializeField;

public class PausePopUp : PopUpUI
{
    private InStageManager _inStageManager;
    private SettingsData _settings;
    
    private bool _isActive;
    public bool IsActive => _isActive;
    
    [SF] private CustomButton resumeBtn;
    [SF] private CustomButton retryBtn;
    [SF] private CustomButton quitBtn;
    
    [SF] private CustomSlider bgmSlider;
    [SF] private CustomSlider sfxSlider;
    
    [SF] private Toggle bgmMuteToggle;
    [SF] private Toggle sfxMuteToggle;

    [SF] private ClipInfo pauseSfx;
    [SF] private ClipInfo unpauseSfx;
    
    protected override void OnEnable()
    {
        base.OnEnable();
        _isActive = true;
        
        ApplyDataToUI();
        SubscribeEvents();
        
        GameManager.Instance.SoundManager.PauseAllSounds(true);
        GameManager.Instance.SoundManager.BuildSfx().WithSfxInfo(pauseSfx).Play();
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
        _settings = GameManager.Instance.SettingsData;
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
        bgmSlider.SubscribeEvent(_settings.SetBgmVolume);
        sfxSlider.SubscribeEvent(_settings.SetSfxVolume);
        OnBgClicked += _inStageManager.ResumeStage;
        
        bgmMuteToggle.onValueChanged.AddListener(OnBgmMuteToggleChanged);
        sfxMuteToggle.onValueChanged.AddListener(OnSfxMuteToggleChanged);
    }

    private void UnsubscribeEvents()
    {
        resumeBtn.UnsubscribeEvent(_inStageManager.ResumeStage);
        retryBtn.UnsubscribeEvent(_inStageManager.RetryStage);
        quitBtn.UnsubscribeEvent(_inStageManager.QuitStage);
        bgmSlider.UnsubscribeEvent(_settings.SetBgmVolume);
        sfxSlider.UnsubscribeEvent(_settings.SetSfxVolume);
        OnBgClicked -= _inStageManager.ResumeStage;
        
        bgmMuteToggle.onValueChanged.RemoveAllListeners();
        sfxMuteToggle.onValueChanged.RemoveAllListeners();
    }

    private void ApplyDataToUI()
    {
        bgmSlider.SyncSliderElements(_settings.BgmVolume);
        sfxSlider.SyncSliderElements(_settings.SfxVolume);
        
        bgmMuteToggle.isOn = _settings.IsBgmMute;
        sfxMuteToggle.isOn = _settings.IsSfxMute;
    }

    private void OnBgmMuteToggleChanged(bool value)
    {
        _settings.SetIsBgmMute(value);
    }

    private void OnSfxMuteToggleChanged(bool value)
    {
        _settings.SetIsSfxMute(value);
    }
}

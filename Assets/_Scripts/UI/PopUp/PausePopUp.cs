using DG.Tweening;
using Sfx;
using UnityEngine;
using UnityEngine.UI;
using SF = UnityEngine.SerializeField;

// [수정!!] 구조가 바뀌어서 Panel_Bg를 클릭할 수 없음 고쳐야....

public class PausePopUp : PopUpUI
{
    [SF] private Canvas popUpCanvas;
    /* UI Elements */
    [Header("[ Buttons ]")]
    [SF] private CustomButton pauseBtn; // [임시] 얘를 어디로 보내야할지...
    [SF] private CustomButton resumeBtn;
    [SF] private CustomButton retryBtn;
    [SF] private CustomButton quitBtn;
    [Header("[ Sliders ]")]
    [SF] private CustomSlider bgmSlider;
    [SF] private CustomSlider sfxSlider;
    [Header("[ Toggles ]")]
    [SF] private Toggle bgmMuteToggle;
    [SF] private Toggle sfxMuteToggle;
    [Header("[ SFX ]")]
    [SF] private SfxInfo pauseSfx;
    [SF] private SfxInfo unpauseSfx;
    /* Fields */
    private StageManager _stageManager;
    private SettingsData _settings;
    private bool _isQuit;
    
    #region Unity Event Methods
    protected override void OnEnable()
    {
        _stageManager?.PauseStage();
        base.OnEnable(); // 요기서 Tween 함
        
        pauseBtn.OnClicked -= PopUp;
        SubscribeEvents();
        GameManager.Instance.SoundManager.PauseAllSounds(true);
        GameManager.Instance.SoundManager.BuildSfx().WithSfxInfo(pauseSfx).Play();

        _isQuit = false;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        
        pauseBtn.OnClicked += PopUp;
        UnsubscribeEvents();
        GameManager.Instance.SoundManager.PauseAllSounds(false);

        if (!_isQuit) popUpCanvas.enabled = false;
    }
    #endregion

    #region Initialize Methods
    public void Init(StageManager sm)
    {
        _stageManager = sm;
        _settings = GameManager.Instance.SettingsData;
        pauseBtn.OnClicked += PopUp;
    }

    private void SubscribeEvents()
    {
        resumeBtn.OnClicked += PopDown;
        resumeBtn.OnClicked += _stageManager.ResumeStage;
        retryBtn.OnClicked += PopDown;
        retryBtn.OnClicked += _stageManager.RetryStage;
        quitBtn.OnClicked += HasQuit;
        quitBtn.OnClicked += PopDown;
        quitBtn.OnClicked += _stageManager.QuitStage;

        bgmSlider.OnValueChanged += _settings.SetBgmVolume;
        sfxSlider.OnValueChanged += _settings.SetSfxVolume;
        
        OnBgClicked += PopDown;
        OnBgClicked += _stageManager.ResumeStage;
        
        bgmMuteToggle.onValueChanged.AddListener(OnBgmMuteToggleChanged);
        sfxMuteToggle.onValueChanged.AddListener(OnSfxMuteToggleChanged);
    }

    private void UnsubscribeEvents()
    {
        resumeBtn.OnClicked -= PopDown;
        resumeBtn.OnClicked -= _stageManager.ResumeStage;
        retryBtn.OnClicked -= PopDown;
        retryBtn.OnClicked -= _stageManager.RetryStage;
        quitBtn.OnClicked -= HasQuit;
        quitBtn.OnClicked -= PopDown;
        quitBtn.OnClicked -= _stageManager.QuitStage;

        bgmSlider.OnValueChanged -= _settings.SetBgmVolume;
        sfxSlider.OnValueChanged -= _settings.SetSfxVolume;
        
        OnBgClicked -= PopDown;
        OnBgClicked -= _stageManager.ResumeStage;
        
        bgmMuteToggle.onValueChanged.RemoveAllListeners();
        sfxMuteToggle.onValueChanged.RemoveAllListeners();
    }
    #endregion

    #region UI Control Methods
    public void PopUp()
    {
        if (IsPopping()) return;
        ApplyDataToUI();
        popUpCanvas.enabled = true;
        ToggleActiveState(true);
    }

    public void PopDown()
    {
        if (IsPopping()) return;
        Pop(0,popUpTweenTargetPosY,Ease.InBack,1f,()=>ToggleActiveState(false));
    }
    
    private void ApplyDataToUI()
    {
        bgmSlider.SyncSliderElements(_settings.BgmVolume);
        sfxSlider.SyncSliderElements(_settings.SfxVolume);
        
        bgmMuteToggle.isOn = _settings.IsBgmMute;
        sfxMuteToggle.isOn = _settings.IsSfxMute;
    }

    private void ToggleActiveState(bool isActive)
    {
        gameObject.SetActive(isActive);
    }
    #endregion
    
    #region UI Event Methods
    private void OnBgmMuteToggleChanged(bool value)
    {
        _settings.SetIsBgmMute(value);
    }

    private void OnSfxMuteToggleChanged(bool value)
    {
        _settings.SetIsSfxMute(value);
    }
    #endregion

    private void HasQuit()
    {
        _isQuit = true;
    }
}

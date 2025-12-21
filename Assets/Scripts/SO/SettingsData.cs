using System;
using UnityEngine;
using SF = UnityEngine.SerializeField;

public enum FrameRate
{
    Opt1 = 30,
    Opt2 = 60,
    Opt3 = 120,
}

public enum GraphicLevel
{
    Low,
    Medium,
    High,
}

[CreateAssetMenu(fileName = "SettingsData", menuName = "SO/UI/Settings")]
public class SettingsData : ScriptableObject
{
    [Header("Sound")] 
    [SF, Range(0,1)] private float bgmVolume;
    [SF, Range(0,1)] private float sfxVolume;
    [SF] private bool isBgmMute;
    [SF] private bool isSfxMute;
    [Header("GamePlay")]
    [SF] private FrameRate currentFrameRate;
    [SF] private GraphicLevel currentGraphicLevel;

    private Action<float> _onBgmVolumeChanged;
    private Action<float> _onSfxVolumeChanged;
    private Action<bool> _onIsBgmMuteChanged;
    private Action<bool> _onIsSfxMuteChanged;
    
    public float BgmVolume => bgmVolume;
    public float SfxVolume => sfxVolume;
    public bool IsBgmMute => isBgmMute;
    public bool IsSfxMute => isSfxMute;
    public FrameRate CurrentFrameRate => currentFrameRate;
    public GraphicLevel CurrentGraphicLevel => currentGraphicLevel;

    public void Init(string json) // 추후 옵션 데이터 불러오면 이걸로 초기화
    {
        // JsonUtility.FromJsonOverwrite(json, this); 
        // 이걸로 간단하게 덮어씌울 수 있다는데?
        // 그리고 json은 오로지 "필드"만 추출한대요 안심
        
        /* 임시 */
        bgmVolume = sfxVolume = 1;
        isBgmMute = true; 
        isSfxMute = false;
        currentFrameRate = FrameRate.Opt3;
        currentGraphicLevel = GraphicLevel.High;
    }

    public void Subscribe(SoundManager manager)
    {
        _onBgmVolumeChanged += manager.OnBgmVolumeChanged;
        _onSfxVolumeChanged += manager.OnSfxVolumeChanged;
        _onIsBgmMuteChanged += manager.OnBgmMuteChanged;
        _onIsSfxMuteChanged += manager.OnSfxMuteChanged;
    }

    public void Unsubscribe(SoundManager manager)
    {
        _onBgmVolumeChanged -= manager.OnBgmVolumeChanged;
        _onSfxVolumeChanged -= manager.OnSfxVolumeChanged;
        _onIsBgmMuteChanged -= manager.OnBgmMuteChanged;
        _onIsSfxMuteChanged -= manager.OnSfxMuteChanged;
    }

    public void SetBgmVolume(float volume)
    {
        volume = Mathf.Clamp01(volume);
        bgmVolume = volume;
        _onBgmVolumeChanged?.Invoke(volume);
    }
    
    public void SetSfxVolume(float volume)
    {
        volume = Mathf.Clamp01(volume);
        sfxVolume = volume;
        _onSfxVolumeChanged?.Invoke(volume);
    }

    public void SetIsBgmMute(bool isMute)
    {
        isBgmMute = isMute;
        _onIsBgmMuteChanged?.Invoke(isMute);
    }

    public void SetIsSfxMute(bool isMute)
    {
        isSfxMute = isMute;
        _onIsSfxMuteChanged?.Invoke(isMute);
    }
}

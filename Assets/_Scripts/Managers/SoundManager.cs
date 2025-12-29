using System.Collections;
using System.Collections.Generic;
using Sfx;
using UnityEngine;
using SF = UnityEngine.SerializeField;

// [참고링크] https://youtu.be/BgpqoRFCNOs?si=WOMq5QCAx7gXHQVM

public class SoundManager : ObjPool<SfxEmitter>
{
    /* Settings */
    private SettingsData _settings;
    /* BGM */
    private AudioSource _bgmAudioSource;
    private Coroutine _coBgmFade;
    [SF] private float bgmFadeSpeed = 1.5f;
    /* SFX */
    // public Queue<SfxEmitter> FrequentSfx { get; private set; }
    private List<SfxEmitter> _activeSfx;
    [SF] private int maxSfx = 30;
    // [메모] Project Settings의 Audio 탭에 Max Real Voices와 Max Virtual Voices가 있음
    // Real은 프로젝트에서 실제로 발생할 수 있는 소리의 개수를 뜻함 (작동 가능한 Audio Source 개수)
    // Virtual은 후보로 대기할 수 있는 소리의 개수를 뜻함 (실제 작동 X)
    // 현재 Play 중인 Audio Source의 Priority를 기준으로 경쟁하게 되고, 값이 작을수록 우선순위가 높음
    
    public void Init(SettingsData data)
    {
        if (!TryGetComponent(out _bgmAudioSource))
            _bgmAudioSource = gameObject.AddComponent<AudioSource>();
        
        _settings = data;
        _settings.Subscribe(this);
        
        InitPool();
        
        _bgmAudioSource.loop = true;
        _bgmAudioSource.volume = _settings.BgmVolume;
        
        _activeSfx = new List<SfxEmitter>(defaultCapacity);
        // FrequentSfx = new Queue<SfxEmitter>(defaultCapacity);
    }
    
    #region Sfx Build Methods
    public SfxBuilder BuildSfx()
    {
        return new SfxBuilder(this);
    }

    public bool CanBuildSfx(ClipInfo info) // [보류] 총소리 같은 거 과도하게 나지 않도록 방지하는 건데, 정상 작동을 안 한다...
    {
        return _activeSfx.Count <= maxSfx;
        // if (!info.isFrequent) return true;
        // if (FrequentSfx.Count < maxSfx) return true;
        // if (!FrequentSfx.TryDequeue(out var sfx)) return true;
        //
        // try
        // {
        //     sfx.Stop();
        //     return true;
        // }   
        // catch
        // {
        //     Debug.Log("sfx is already stopped");
        //     return false;
        // }
    }
    #endregion

    #region Settings Event Methods
    public void OnBgmVolumeChanged(float volume)
    {
        _bgmAudioSource.volume = volume;
    }

    public void OnBgmMuteChanged(bool isMute)
    {
        _bgmAudioSource.mute = isMute;
    }
    
    public void OnSfxVolumeChanged(float volume)
    {
        foreach (SfxEmitter sfx in _activeSfx)     
        {
            sfx.AudioSource.volume = volume;
        }
    }

    public void OnSfxMuteChanged(bool isMute)
    {
        foreach (SfxEmitter sfx in _activeSfx)     
        {
            sfx.AudioSource.mute = isMute;
        }
    }
    #endregion
    
    #region Sound Control Methods
    public void PauseAllSounds(bool isPaused)
    {
        foreach (SfxEmitter sfx in _activeSfx)     
        {
            if (isPaused) sfx.AudioSource.Pause();
            else sfx.AudioSource.UnPause();
        }
        
        if (isPaused) _bgmAudioSource.Pause();
        else _bgmAudioSource.UnPause();
    }

    public void TurnOffActiveSfx()
    {
        /* 컬렉션 순회 삭제는 "역순 for문"인 이유 */
        // "foreach"는 열거자를 활용하지비. 그리고 이 열거자엔 순회 시작 시 컬렉션의 Version이 기록돼이씀
        // Version은 컬렉션에 Remove나 Add가 호출될 때마다 변하는데,
        // 순회 도중 요소나 구성이 변하면 이 Version이 달라지며,
        // foreach는 현재 순회에 대한 신뢰성을 잃고 InvalidOperation 예외를 내버림
        // "정순 for문"은 현재 순회 대상이 List기 때문에 문제.
        // List는 요소가 사라지면 그 이후 요소들의 인덱스를 당겨와버림.
        // for문은 인덱스를 기준으로 순회하기 때문에, 요소별 인덱스가 변하며,
        // 순회 대상이 누락되거나, 중복되거나, 또는 범위를 벗어나 OutOfRange 예외를 내버릴 수 이씀
        for (int i = _activeSfx.Count-1; i >= 0; i--)
        { 
            _activeSfx[i].Stop();
        }
        
        // FrequentSfx.Clear();
    }

    public void TurnOffCurrentBgm(bool immediately = false)
    {
        if (_bgmAudioSource.clip is null) return;
        if (immediately)
        {
            _bgmAudioSource.Stop();
            _bgmAudioSource.clip = null;
            return;
        }
        StartCoroutine(FadeOutBgm());
    }
    
    public void ChangeBgm(ClipInfo info)
    {
        if (info.clip is null) return;
        _bgmAudioSource.clip = info.clip;
        _bgmAudioSource.volume = _settings.BgmVolume; // Fade에서 volume을 0으로 낮춰서, 여기서 다시 초기화해줘야해
        StartCoroutine(FadeInBgm());
    }

    private IEnumerator FadeOutBgm()
    {
        while (_bgmAudioSource.volume > 0)
        {
            _bgmAudioSource.volume -= _settings.BgmVolume * bgmFadeSpeed * Time.unscaledDeltaTime;
            yield return null;
        }
        
        _bgmAudioSource.Stop();
        _bgmAudioSource.clip = null;
    }

    private IEnumerator FadeInBgm()
    {
        _bgmAudioSource.Play();
        
        while (_bgmAudioSource.volume < _settings.BgmVolume)
        {
            _bgmAudioSource.volume += _settings.BgmVolume * bgmFadeSpeed * Time.unscaledDeltaTime;
            yield return null;
        }
    }
    #endregion

    #region Pool Methods
    public SfxEmitter GetSfx()
    {
        return Pool.Get();
    }

    public void ReleaseSfx(SfxEmitter sfx)
    {
        Pool.Release(sfx);
    }

    protected override SfxEmitter CreateObj()
    {
        SfxEmitter sfx = base.CreateObj();
        objIdx += 1;
        sfx.AudioSource.playOnAwake = sfx.AudioSource.loop = false;
        sfx.AudioSource.volume = _settings.SfxVolume;
        return sfx;
    }

    protected override void OnGot(SfxEmitter obj)
    {
        if (isPrewarming) return;
        base.OnGot(obj);
        obj.Init(_settings.SfxVolume, _settings.IsSfxMute);
        _activeSfx.Add(obj);
    }

    protected override void OnReleased(SfxEmitter obj)
    {
        if (isPrewarming) return;
        base.OnReleased(obj);
        _activeSfx.Remove(obj);
    }
    #endregion
}

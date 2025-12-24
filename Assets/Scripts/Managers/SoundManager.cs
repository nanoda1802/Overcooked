using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SF = UnityEngine.SerializeField;

public class SoundManager : MonoBehaviour, IPool<AudioSource>
{
    private SettingsData _settingsInfo;
    
    [SF] private AudioSource bgm;
    [SF] private int poolSize;
    private Queue<AudioSource> _pool;

    [SF] private float volumeFadeSpeed = 1.5f;
    [SF] private AudioClip btnDefaultSoundClip;

    private List<AudioSource> _activeSfx;

    public void Init(SettingsData data)
    {
        _settingsInfo = data;
        _settingsInfo.Subscribe(this);
        InitPool();
        _activeSfx = new List<AudioSource>(poolSize);

        OnBgmMuteChanged(_settingsInfo.IsBgmMute);
        OnSfxMuteChanged(_settingsInfo.IsSfxMute);
    }

    public void InitPool()
    {
        _pool = new Queue<AudioSource>(poolSize);

        for (int i = 0; i < poolSize; i++)
        {
            AudioSource sfx = InstantiateSfxObj(i);
            ReturnToPool(sfx);
        }
    }

    public bool TryGetItem(out AudioSource poolable) // [버그 발생] 확실하진 않은데 이거 블로킹 발생하는 듯? 순서 잘 정해줘야...?
    {
        if (_pool.TryDequeue(out poolable))
        {
            poolable.gameObject.SetActive(true);
            return true;
        }

        poolable = InstantiateSfxObj();
        return true;
    }
    
    public void ReturnToPool(AudioSource poolable)
    {
        poolable.loop = false;
        poolable.clip = null;
        poolable.gameObject.SetActive(false);
        _pool.Enqueue(poolable);
    }

    private AudioSource InstantiateSfxObj(int idx = -1)
    {
        string sfxName = idx >= 0 ? $"Sfx_{idx}" : "Sfx_Instant";
        GameObject sfxObj = new GameObject(sfxName);
        sfxObj.transform.SetParent(transform);
        
        AudioSource audioSource = sfxObj.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        return audioSource;
    }

    public void OnBgmVolumeChanged(float volume)
    {
        bgm.volume = volume;
    }

    public void OnBgmMuteChanged(bool isMute)
    {
        bgm.mute = isMute;
    }

    public void OnSfxVolumeChanged(float volume)
    {
        foreach (AudioSource sfx in _activeSfx)     
        {
            sfx.volume = volume;
        }
    }

    public void OnSfxMuteChanged(bool isMute)
    {
        foreach (AudioSource sfx in _activeSfx)
        {
            sfx.mute = isMute;
        }
    }

    public void PauseAllSounds(bool isPaused)
    {
        foreach (AudioSource sfx in _activeSfx)
        {
            if (isPaused) sfx.Pause();
            else sfx.UnPause();
        }

        if (isPaused) bgm.Pause();
        else bgm.UnPause();
    }

    public void TurnOffCurrentBgm(bool immediately = false)
    {
        if (bgm.clip is null) return;
        if (immediately)
        {
            bgm.Stop();
            bgm.clip = null;
            return;
        }
        StartCoroutine(CoFadeOutBgm());
    }

    public void ChangeBgm(AudioClip clip)
    {
        if (clip is null) return;
        bgm.clip = clip;
        bgm.volume = _settingsInfo.BgmVolume;
        StartCoroutine(CoFadeInBgm());
    }

    private IEnumerator CoFadeOutBgm()
    {
        while (bgm.volume > 0)
        {
            bgm.volume -= Time.unscaledDeltaTime * volumeFadeSpeed * _settingsInfo.BgmVolume;
            yield return null;
        }
        
        bgm.Stop();
        bgm.clip = null;
    }

    private IEnumerator CoFadeInBgm()
    {
        bgm.Play();

        while (bgm.volume < _settingsInfo.BgmVolume)
        {
            bgm.volume += Time.unscaledDeltaTime * volumeFadeSpeed * _settingsInfo.BgmVolume;
            yield return null;
        }
    }

    public void TurnOffAllSfx()
    {
        foreach (AudioSource sfx in _activeSfx)
        {
            sfx.Stop();
            ReturnToPool(sfx);
        }
        
        _activeSfx.Clear();
    }

    public void PlaySfx(AudioClip clip = null)
    {
        TryGetItem(out AudioSource sfx);
        sfx.clip = clip is null ? btnDefaultSoundClip : clip;
        sfx.volume = _settingsInfo.SfxVolume;
        sfx.mute = _settingsInfo.IsSfxMute;
        
        _activeSfx.Add(sfx);
        StartCoroutine(CoPlaySfx(sfx));
    }

    private IEnumerator CoPlaySfx(AudioSource sfx)
    {
        sfx.Play();
        yield return new WaitForSeconds(sfx.clip.length); // [임시]

        while (sfx.isPlaying) // 혹시 모를 잔여 재생 대기
        {
            yield return null;
        }

        _activeSfx.Remove(sfx);
        ReturnToPool(sfx);
    }

    public AudioSource PlayLoopingSfx(AudioClip clip)
    {
        TryGetItem(out AudioSource sfx);
        sfx.clip = clip;
        sfx.loop = true;
        sfx.mute = _settingsInfo.IsSfxMute;
        sfx.volume = _settingsInfo.SfxVolume; 
        
        _activeSfx.Add(sfx);
        sfx.Play();
        return sfx;
    }

    public void TurnOffLoopingSfx(AudioSource sfx)
    {
        if (sfx is null) return;
        StartCoroutine(CoFadeOutLoopingSfx(sfx));
    }

    private IEnumerator CoFadeOutLoopingSfx(AudioSource sfx)
    {
        while (sfx.volume > 0)
        {
            sfx.volume -= Time.unscaledDeltaTime * volumeFadeSpeed * _settingsInfo.SfxVolume;
            yield return null;
        }
        
        sfx.Stop();
        _activeSfx.Remove(sfx);
        ReturnToPool(sfx);
    }
}

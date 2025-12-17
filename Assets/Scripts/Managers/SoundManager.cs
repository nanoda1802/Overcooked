using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SF = UnityEngine.SerializeField;

public class SoundManager : MonoBehaviour, IPool<AudioSource>
{
    [SF] private AudioSource bgm;
    [SF] private int poolSize;
    private Queue<AudioSource> _pool;
    
    [SF] private float sfxMaxVolume = 1f;
    [SF] private AudioClip btnDefaultSoundClip;
    
    public void InitPool()
    {
        _pool = new Queue<AudioSource>(poolSize);

        for (int i = 0; i < poolSize; i++)
        {
            AudioSource sfx = InstantiateSfxObj(i);
            ReturnToPool(sfx);
        }
    }

    public bool TryGetItem(out AudioSource poolable)
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

    public void MuteCurrentBgm()
    {
        if (bgm.clip is null) return;
        StartCoroutine(CoFadeOutBgm());
    }

    public void ChangeBgm(AudioClip clip)
    {
        if (clip is null) return;
        StartCoroutine(CoFadeInBgm(clip));
    }

    private IEnumerator CoFadeOutBgm()
    {
        while (bgm.volume > 0)
        {
            bgm.volume -= Time.unscaledDeltaTime * 1.5f;
            yield return null;
        }
        
        bgm.Stop();
    }

    private IEnumerator CoFadeInBgm(AudioClip clip)
    {
        bgm.clip = clip;
        bgm.Play();

        while (bgm.volume < sfxMaxVolume)
        {
            bgm.volume += Time.unscaledDeltaTime * 1.5f;
            yield return null;
        }
    }

    public void PlaySfx(AudioClip clip = null)
    {
        TryGetItem(out AudioSource sfx);
        sfx.clip = clip is null ? btnDefaultSoundClip : clip;
        sfx.volume = 1; // [임시] 추후 옵션으로 일괄 ...
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
        
        ReturnToPool(sfx);
    }

    public AudioSource PlayLoopingSfx(AudioClip clip)
    {
        TryGetItem(out AudioSource sfx);
        sfx.clip = clip;
        sfx.loop = true;
        sfx.volume = 1; // [임시] 추후 옵션으로 일괄 ... 
        sfx.Play();
        return sfx;
    }

    public void MuteLoopingSfx(AudioSource sfx)
    {
        if (sfx is null) return;
        StartCoroutine(CoFadeOutLoopingSfx(sfx));
    }

    private IEnumerator CoFadeOutLoopingSfx(AudioSource sfx)
    {
        while (sfx.volume > 0)
        {
            sfx.volume -= Time.unscaledDeltaTime * 1.5f;
            yield return null;
        }
        
        sfx.Stop();
        ReturnToPool(sfx);
    }
}

using System.Collections;
using Sfx;
using UnityEngine;

public class SfxEmitter : MonoBehaviour
{
    // private ClipInfo _clipInfo;
    public AudioSource AudioSource => _audioSource;
    private AudioSource _audioSource;
    private Coroutine _coPlaying;

    private void Awake()
    {
        if (!TryGetComponent(out _audioSource))
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void Play()
    {
        if (_coPlaying is not null) StopCoroutine(_coPlaying);
        _audioSource.Play();
        if (!_audioSource.loop) _coPlaying = StartCoroutine(WaitAndReturn());
    }

    public void Stop()
    {
        if (_coPlaying is not null)
        {
            StopCoroutine(_coPlaying);
            _coPlaying = null;
        }
        
        _audioSource.Stop();
        GameManager.Instance.SoundManager.ReleaseSfx(this);
    }

    private IEnumerator WaitAndReturn()
    {
        float clipLen = _audioSource.clip.length;

        do
        {
            clipLen -= Time.unscaledDeltaTime;
            yield return null;
        } 
        while (clipLen > 0);
        
        GameManager.Instance.SoundManager.ReleaseSfx(this);
    }

    public void Init(float volume, bool isMute)
    {
        _audioSource.volume = volume;
        _audioSource.pitch = 1;
        _audioSource.mute = isMute;
    }

    public void ApplyClipInfo(ClipInfo info)
    {
        // _clipInfo = info;
        _audioSource.clip = info.clip;
        _audioSource.outputAudioMixerGroup = info.mixerGroup;
        _audioSource.loop = info.isLoop;
    }
    
    public void SetPos(Vector3 pos, Transform parent)
    {
        transform.position = pos;
        transform.SetParent(parent);
    }
    
    public void ApplyRandomPitch(float min = -0.05f, float max = 0.05f)
    {
        _audioSource.pitch += Random.Range(min, max);
    }
}

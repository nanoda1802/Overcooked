using System.Collections;
using UnityEngine;

public class SoundEmitter : MonoBehaviour
{
    public SoundDataasdf SoundDataasdf { get; private set; }
    private AudioSource audioSource;
    private Coroutine playingCoroutine;

    private WaitWhile waitWhile;
    
    private void Awake()
    {
        if (!TryGetComponent(out audioSource))
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void Play()
    {
        if (playingCoroutine != null) StopCoroutine(playingCoroutine);
        audioSource.Play();
        playingCoroutine = StartCoroutine(WaitForSoundToEnd());
    }

    public void Stop()
    {
        if (playingCoroutine != null)
        {
            StopCoroutine(playingCoroutine);
            playingCoroutine = null;
        }
        
        audioSource.Stop();
        SfxManager.Instance.ReleaseSfx(this);
    }

    private IEnumerator WaitForSoundToEnd()
    {
        yield return new WaitWhile(()=>audioSource.isPlaying);
        yield return waitWhile;
        SfxManager.Instance.ReleaseSfx(this);
    }

    private void ppp()
    {
        waitWhile = new WaitWhile(()=>audioSource.isPlaying);
    }


    public void Init(SoundDataasdf dataasdf)
    {
        SoundDataasdf = dataasdf;
        audioSource.clip = dataasdf.clip;
        audioSource.outputAudioMixerGroup = dataasdf.mixerGroup;
        audioSource.loop = dataasdf.loop;
        audioSource.playOnAwake = dataasdf.playOnAwake;
    }

    public void WithRandomPitch(float min = -0.05f, float max = 0.05f)
    {
        audioSource.pitch += Random.Range(min, max);
    }
}

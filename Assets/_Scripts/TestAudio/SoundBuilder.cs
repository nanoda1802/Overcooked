using UnityEngine;

public class SoundBuilder
{
    readonly SfxManager sfxManager;
    SoundDataasdf _soundDataasdf;
    private Vector3 pos = Vector3.zero;
    private bool randomPitch;
    
    public SoundBuilder(SfxManager sfxManager)
    {
        this.sfxManager = sfxManager;
    }

    public SoundBuilder WithSoundData(SoundDataasdf soundDataasdf)
    {
        this._soundDataasdf = soundDataasdf;
        return this;
    }

    public SoundBuilder WithPos(SoundDataasdf soundDataasdf)
    {
        this._soundDataasdf = soundDataasdf;
        return this;
    }

    public SoundBuilder WithRandomPitch()
    {
        this.randomPitch = true;
        return this;
    }

    public void Play()
    {
        if (!sfxManager.CanPlaySfx(_soundDataasdf)) return;

        SoundEmitter soundEmitter = sfxManager.GetSfx();
        soundEmitter.Init(_soundDataasdf);
        soundEmitter.transform.position = pos;
        soundEmitter.transform.SetParent(sfxManager.transform);

        if (randomPitch)
        {
            soundEmitter.WithRandomPitch();
        }

        if (_soundDataasdf.isFrequent)
        {
            sfxManager.FrequentSoundEmitters.Enqueue(soundEmitter);
        }
        
        soundEmitter.Play();
    }
}

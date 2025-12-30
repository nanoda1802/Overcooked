using Sfx;
using UnityEngine;

public class SfxBuilder
{
    private readonly SoundManager _soundManager;
    private SfxInfo _sfxInfo;
    private Vector3 _pos = Vector3.zero;
    private bool _hasClipInfo;
    
    private bool _randomPitch;

    public SfxBuilder(SoundManager sm)
    {
        _soundManager = sm;
    }

    public SfxBuilder WithSfxInfo(SfxInfo info)
    {
        _sfxInfo = info;
        _hasClipInfo = true;
        return this;
    }

    public SfxBuilder WithPos(Vector3 pos)
    {
        _pos = pos;
        return this;
    }

    public SfxBuilder WithRandomPitch()
    {
        _randomPitch = true;
        return this;
    }

    public SfxEmitter Play()
    {
        if (!_hasClipInfo) return null;
        if (_sfxInfo.Clip is null) return null;
        // if (!_soundManager.CanBuildSfx(_clipInfo)) return null;
        
        SfxEmitter sfx = _soundManager.GetSfx();
        sfx.ApplyClipInfo(_sfxInfo);
        sfx.SetPos(_pos, _soundManager.transform);
        
        if (_randomPitch) sfx.ApplyRandomPitch();
        // if (_clipInfo.isFrequent) _soundManager.FrequentSfx.Enqueue(sfx);
        
        sfx.Play();
        return sfx;
    }
}

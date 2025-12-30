using System;
using Sfx;
using UnityEngine;
using UnityEngine.UI;
using SF = UnityEngine.SerializeField;

public class WorkTable : PlaceTable
{
    private bool _isWorking;
    
    protected event Action OnFinished;
    protected event Action OnStopped;
    
    [SF] protected Canvas fillBarCanvas;
    [SF] protected Image[] barImages;
    [SF] protected SfxInfo workSfx;
    [SF] protected ParticleSystem workVfx;
    
    [SF] private string animParamName;
    protected int animHash;

    protected virtual void Start() // [임시] 초기화해주는 함수 만들기... 근데 누가 테이블들을 초기화해줌?
    {
        animHash = Animator.StringToHash(animParamName);
    }

    protected void Update()
    {
        if (!_isWorking) return;
        if (placedItem is null) return;
        
        Work();
    }
    
    public virtual bool BeginWork(PlayerController_Stage player = null)
    {
        _isWorking = true;
        
        SfxEmitter curSfx = GameManager.Instance.SoundManager.BuildSfx().WithSfxInfo(workSfx).WithPos(transform.position).Play();
        if (curSfx is not null) OnStopped += curSfx.Stop;
        if (workVfx is not null) OnStopped += StopVfxSmoothly;
        PlayVfx();
        
        return true;
    }

    protected virtual void StopWork()
    {
        _isWorking = false;
        OnStopped?.Invoke();
        OnStopped = OnFinished = null;
    }

    protected virtual void FinishWork()
    {
        OnFinished?.Invoke();
        StopWork();
    }

    private void Work()
    {
        FillBarImg(placedItem.Handle());
        if (placedItem.IsMaxDone()) FinishWork();
    }
    
    private void PlayVfx()
    {
        if (workVfx is null) return;
        if (workVfx.isPlaying) StopVfxSmoothly();
        workVfx.Play();
    }

    private void StopVfxSmoothly()
    {
        // StopEmitting : 추가 파티클만 막음, 이미 나온 녀석들은 남아서 마저 진행됨
        // StopEmittingAndClear : 아예 모든 파티클 제거
        workVfx?.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }

    protected void ActivateUI()
    {
        fillBarCanvas?.gameObject.SetActive(true);
        foreach (Image img in barImages) img.fillAmount = 0;
    }

    protected void DeactivateUI()
    {
        fillBarCanvas?.gameObject.SetActive(false);
    }
    
    private void FillBarImg(float ratio)
    {
        if (ratio >= barImages.Length) return;
        barImages[(int)ratio].fillAmount = ratio % 1;
    }
}

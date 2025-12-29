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
    [SF] protected ClipInfo workSfx;
    
    // [SF] protected SfxEmitter curSfx;
    
    protected void Update()
    {
        if (!_isWorking) return;
        if (placedItem is null) return;
        
        Work();
    }
    
    public virtual bool BeginWork(InStagePlayerController player = null)
    {
        _isWorking = true;
        SfxEmitter curSfx = GameManager.Instance.SoundManager.BuildSfx().WithSfxInfo(workSfx).WithPos(transform.position).Play();
        if (curSfx is not null) OnStopped += curSfx.Stop;
        return true;
    }

    protected virtual void StopWork()
    {
        _isWorking = false;
        OnStopped?.Invoke();
        // GameManager.Instance.SoundManager.TurnOffLoopingSfx(curSfx);
        // curSfx = null;
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

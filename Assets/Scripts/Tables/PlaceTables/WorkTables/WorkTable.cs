using UnityEngine;
using UnityEngine.UI;
using SF = UnityEngine.SerializeField;

public class WorkTable : PlaceTable
{
    private bool _isWorking;
    [SF] protected Canvas fillBarCanvas;
    [SF] protected Image[] barImages;
    [SF] protected AudioClip workSoundClip;
    
    [SF] protected AudioSource curSfx;
    
    protected void Update()
    {
        if (!_isWorking) return;
        if (placedItem is null) return;
        
        Work();
    }
    
    public virtual bool BeginWork(InStagePlayerController player = null)
    {
        _isWorking = true;
        // [sfx] 테이블 별 일하는 소리 시작
        curSfx = GameManager.Instance.SoundManager.PlayLoopingSfx(workSoundClip);
        return true;
    }

    protected virtual void StopWork()
    {
        _isWorking = false;
        // [sfx] 테이블 별 일하는 소리 끝
        GameManager.Instance.SoundManager.TurnOffLoopingSfx(curSfx);
        curSfx = null;
    }

    protected virtual void FinishWork()
    {
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

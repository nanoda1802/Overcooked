using UnityEngine;
using UnityEngine.UI;
using SF = UnityEngine.SerializeField;

public class WorkTable : PlaceTable
{
    protected bool IsWorking;
    [SF] protected Canvas fillBarCanvas;
    [SF] protected Image[] barImages;

    protected void Update()
    {
        if (!IsWorking) return;
        if (placedItem is null) return;
        
        Work();
    }
    
    public virtual bool BeginWork(PlayerController player)
    {
        if (placedItem is null) return false;
        return IsWorking = true;
    }

    protected virtual void StopWork()
    {
        IsWorking = false;
    }

    protected virtual void FinishWork()
    {
        IsWorking = false;
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
